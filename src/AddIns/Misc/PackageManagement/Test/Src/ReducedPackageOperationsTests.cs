﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ReducedPackageOperationsTests
	{
		ReducedPackageOperations reducedPackageOperations;
		IPackageOperationResolver fakePackageOperationResolver;
		List<IPackage> packages;
		
		void CreateReducedPackageOperations()
		{
			packages = new List<IPackage>();
			fakePackageOperationResolver = MockRepository.GenerateStub<IPackageOperationResolver>();
			reducedPackageOperations = new ReducedPackageOperations(fakePackageOperationResolver, packages);
		}
		
		IPackage AddPackage(string id, string version)
		{
			IPackage package = CreatePackage(id, version);
			packages.Add(package);
			
			return package;
		}
		
		IPackage CreatePackage(string id, string version)
		{
			IPackage package = MockRepository.GenerateStub<IPackage>();
			package.Stub(p => p.Id).Return(id);
			package.Stub(p => p.Version).Return(new SemanticVersion(version));
			return package;
		}
		
		PackageOperation AddInstallOperationForPackage(IPackage package)
		{
			var operation = new PackageOperation(package, PackageAction.Install);
			AddInstallOperationsForPackage(package, operation);
			return operation;
		}
		
		void AddInstallOperationsForPackage(IPackage package, params PackageOperation[] operations)
		{
			fakePackageOperationResolver
				.Stub(resolver => resolver.ResolveOperations(package))
				.Return(operations);
		}
		
		PackageOperation CreatePackageOperation(string id, string version, PackageAction action)
		{
			IPackage package = CreatePackage(id, version);
			return new PackageOperation(package, action);
		}
		
		void AssertReducedOperationsContains(PackageOperation operation)
		{
			Assert.Contains(operation, reducedPackageOperations.Operations.ToList());
		}
		
		[Test]
		public void Reduce_OnePackage_ReturnsPackageOperationsFromResolverForPackage()
		{
			CreateReducedPackageOperations();
			IPackage package = AddPackage("Test", "1.0");
			PackageOperation operation = AddInstallOperationForPackage(package);
			
			reducedPackageOperations.Reduce();
			
			Assert.AreEqual(1, reducedPackageOperations.Operations.Count());
			Assert.AreEqual(operation, reducedPackageOperations.Operations.First());
		}
		
		[Test]
		public void Reduce_TwoPackages_ReturnsPackageOperationsForBothPackages()
		{
			CreateReducedPackageOperations();
			IPackage package1 = AddPackage("Test", "1.0");
			IPackage package2 = AddPackage("Test2", "1.0");
			PackageOperation operation1 = AddInstallOperationForPackage(package1);
			PackageOperation operation2 = AddInstallOperationForPackage(package2);
			
			reducedPackageOperations.Reduce();
			
			Assert.AreEqual(2, reducedPackageOperations.Operations.Count());
			AssertReducedOperationsContains(operation1);
			AssertReducedOperationsContains(operation2);
		}
		
		[Test]
		public void Reduce_OncePackageOperationInstallsPackageWhilstOneUninstallsSamePackage_PackageOperationNotIncludedInReducedSet()
		{
			CreateReducedPackageOperations();
			IPackage package = AddPackage("Test", "1.0");
			PackageOperation installOperation = CreatePackageOperation("Foo", "1.0", PackageAction.Install);
			PackageOperation uninstallOperation = CreatePackageOperation("Foo", "1.0", PackageAction.Uninstall);
			AddInstallOperationsForPackage(package, installOperation, uninstallOperation);
			
			reducedPackageOperations.Reduce();
			
			Assert.AreEqual(0, reducedPackageOperations.Operations.Count());
		}
		
		[Test]
		public void Reduce_OnePackageOperationMatchesPackageBeingInstalled_ReturnsOnlyOnePackageInstallOperationForThisPackage()
		{
			CreateReducedPackageOperations();
			IPackage package1 = AddPackage("Test", "1.0");
			IPackage package2 = AddPackage("Test2", "1.0");
			PackageOperation operation1a = CreatePackageOperation("Test", "1.0", PackageAction.Install);
			PackageOperation operation1b = CreatePackageOperation("Test2", "1.0", PackageAction.Install);
			PackageOperation operation2 = CreatePackageOperation("Test2", "1.0", PackageAction.Install);
			AddInstallOperationsForPackage(package1, operation1a, operation1b);
			AddInstallOperationsForPackage(package2, operation2);
			
			reducedPackageOperations.Reduce();
			
			PackageOperation operation = reducedPackageOperations
				.Operations
				.SingleOrDefault(o => o.Package.Id == "Test2");
			Assert.AreEqual(2, reducedPackageOperations.Operations.Count());
		}
		
		[Test]
		public void Reduce_OnePackageOperationMatchesPackageBeingInstalledOnlyById_MatchingPackageOperationByIdIncludedInSet()
		{
			CreateReducedPackageOperations();
			IPackage package1 = AddPackage("Test", "1.0");
			IPackage package2 = AddPackage("Test2", "1.0");
			PackageOperation operation1a = CreatePackageOperation("Test", "1.0", PackageAction.Install);
			PackageOperation operation1b = CreatePackageOperation("Test2", "1.1", PackageAction.Install);
			PackageOperation operation2 = CreatePackageOperation("Test2", "1.0", PackageAction.Install);
			AddInstallOperationsForPackage(package1, operation1a, operation1b);
			AddInstallOperationsForPackage(package2, operation2);
			
			reducedPackageOperations.Reduce();
			
			Assert.AreEqual(3, reducedPackageOperations.Operations.Count());
		}
		
		[Test]
		public void Reduce_OnePackageOperationMatchesPackageBeingInstalledByIdAndVersionButOneIsInstallAndOneIsUninstall_BothOperationsNotIncludedInSet()
		{
			CreateReducedPackageOperations();
			IPackage package1 = AddPackage("Test", "1.0");
			IPackage package2 = AddPackage("Test2", "1.0");
			PackageOperation operation1a = CreatePackageOperation("Test", "1.0", PackageAction.Install);
			PackageOperation operation1b = CreatePackageOperation("Test2", "1.0", PackageAction.Uninstall);
			PackageOperation operation2 = CreatePackageOperation("Test2", "1.0", PackageAction.Install);
			AddInstallOperationsForPackage(package1, operation1a, operation1b);
			AddInstallOperationsForPackage(package2, operation2);
			
			reducedPackageOperations.Reduce();
			
			PackageOperation operation = reducedPackageOperations
				.Operations
				.SingleOrDefault(o => o.Package.Id == "Test");
			Assert.AreEqual(1, reducedPackageOperations.Operations.Count());
		}
	}
}