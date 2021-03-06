﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that an array is converted from C# to Ruby.
	/// </summary>
	[TestFixture]
	public class ArrayConversionTestFixture
	{		
		string integerArray = "class Foo\r\n" +
						"{\r\n" +
						"    public int[] Run()\r\n" +
						"    {" +
						"        int[] i = new int[] {1, 2, 3, 4};\r\n" +
						"        i[0] = 5;\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";

		string int32Array = "class Foo\r\n" +
						"{\r\n" +
						"    public Int32[] Run()\r\n" +
						"    {" +
						"        Int32[] i = new Int32[] {1, 2, 3, 4};\r\n" +
						"        i[0] = 5;\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";
		
		string uintArray = "class Foo\r\n" +
						"{\r\n" +
						"    public uint[] Run()\r\n" +
						"    {" +
						"        uint[] i = new uint[] {1, 2, 3, 4};\r\n" +
						"        i[0] = 5;\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";
		
		string stringArray = "class Foo\r\n" +
						"{\r\n" +
						"    public string[] Run()\r\n" +
						"    {" +
						"        string[] i = new string[] {\"a\", \"b\"};\r\n" +
						"        i[0] = \"c\";\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";

		string barArray = "class Foo\r\n" +
						"{\r\n" +
						"    public string[] Run()\r\n" +
						"    {" +
						"        Bar[] i = new Bar[] { new Bar(), new Bar()};\r\n" +
						"        i[0] = new Bar();\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";

		string uriArray = "class Foo\r\n" +
						"{\r\n" +
						"    public Uri[] Run()\r\n" +
						"    {" +
						"        Uri[] i = new Uri[] {new Uri(\"a\"), new Uri(\"b\")};\r\n" +
						"        i[0] = new Uri(\"c\");\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedIntegerArrayCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        i = Array[System::Int32].new([1, 2, 3, 4])\r\n" +
				"        i[0] = 5\r\n" +
				"        return i\r\n" +
				"    end\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(integerArray);
			
			Assert.AreEqual(expectedCode, code);
		}	
		
		[Test]
		public void ConvertedStringArrayCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        i = Array[System::String].new([\"a\", \"b\"])\r\n" +
				"        i[0] = \"c\"\r\n" +
				"        return i\r\n" +
				"    end\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(stringArray);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ConvertedBarArrayCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        i = Array[Bar].new([Bar.new(), Bar.new()])\r\n" +
				"        i[0] = Bar.new()\r\n" +
				"        return i\r\n" +
				"    end\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(barArray);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ConvertedUriArrayCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        i = Array[Uri].new([Uri.new(\"a\"), Uri.new(\"b\")])\r\n" +
				"        i[0] = Uri.new(\"c\")\r\n" +
				"        return i\r\n" +
				"    end\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(uriArray);
			
			Assert.AreEqual(expectedCode, code);
		}		
		
		[Test]
		public void ConvertedUIntArrayCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        i = Array[System::UInt32].new([1, 2, 3, 4])\r\n" +
				"        i[0] = 5\r\n" +
				"        return i\r\n" +
				"    end\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(uintArray);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ConvertedInt32ArrayCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        i = Array[Int32].new([1, 2, 3, 4])\r\n" +
				"        i[0] = 5\r\n" +
				"        return i\r\n" +
				"    end\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(int32Array);
			
			Assert.AreEqual(expectedCode, code);
		}		
	}
}
