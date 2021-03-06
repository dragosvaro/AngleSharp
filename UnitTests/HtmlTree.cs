﻿using AngleSharp;
using AngleSharp.DOM;
using AngleSharp.DOM.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class HtmlTree
    {
        [TestMethod]
        public void TreeHasOneBangComment()
        {
            var doc = DocumentBuilder.Html("<!-- BANG IT --!>");
            Assert.AreEqual(2, doc.ChildNodes.Length);
        }

        [TestMethod]
        public void TreeNonConformingTable()
        {
            //8.2.5.4.7 The "in body" insertion mode - "In the non-conforming ..."
            var doc = DocumentBuilder.Html(@"<a href=""a"">a<table><a href=""b"">b</table>x");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new HTMLAnchorElement().SetAttribute("href", "a"))
                        .AppendChild(new TextNode("a")).ParentNode
                        .AppendChild(new HTMLAnchorElement().SetAttribute("href", "b"))
                            .AppendChild(new TextNode("b")).ParentNode.ParentNode
                        .AppendChild(new HTMLTableElement()).ParentNode.ParentNode
                    .AppendChild(new HTMLAnchorElement().SetAttribute("href", "b"))
                        .AppendChild(new TextNode("x")).ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }

        [TestMethod]
        public void TreeOneTextNodeTableBeforeABCD()
        {
            //One Text node before the table, containing "ABCD"
            var doc = DocumentBuilder.Html(@"A<table>B<tr>C</tr>D</table>");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new TextNode("ABCD")).ParentNode
                    .AppendChild(new HTMLTableElement())
                        .AppendChild(new HTMLTableSectionElement())
                            .AppendChild(new HTMLTableRowElement()).ParentNode.ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }

        [TestMethod]
        public void TreeOneTextNodeTableBeforeAspaceBspaceC()
        {
            //One Text node before the table, containing "A B C" (A-space-B-space-C).
            var doc = DocumentBuilder.Html(@"A<table><tr> B</tr> C</table>");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new TextNode("A B C")).ParentNode
                    .AppendChild(new HTMLTableElement())
                        .AppendChild(new HTMLTableSectionElement())
                            .AppendChild(new HTMLTableRowElement()).ParentNode.ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }

        [TestMethod]
        public void TreeOneTextNodeTableBeforeAspaceBC()
        {
            //One Text node before the table, containing "A BC" (A-space-B-C), and one Text node inside the table (as a child of a tbody) with a single space character.
            var doc = DocumentBuilder.Html(@"A<table><tr> B</tr> </em>C</table>");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new TextNode("A BC")).ParentNode
                    .AppendChild(new HTMLTableElement())
                        .AppendChild(new HTMLTableSectionElement())
                            .AppendChild(new HTMLTableRowElement()).ParentNode
                            .AppendChild(new TextNode(" ")).ParentNode.ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }

        [TestMethod]
        public void TreeUnexpectedTableMarkup()
        {
            //8.2.8.3 Unexpected markup in tables
            var doc = DocumentBuilder.Html(@"<table><b><tr><td>aaa</td></tr>bbb</table>ccc");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new HTMLElement() { NodeName = "b" }).ParentNode
                    .AppendChild(new HTMLElement() { NodeName = "b" })
                        .AppendChild(new TextNode("bbb")).ParentNode.ParentNode
                    .AppendChild(new HTMLTableElement())
                        .AppendChild(new HTMLTableSectionElement())
                            .AppendChild(new HTMLTableRowElement())
                                .AppendChild(new HTMLTableCellElement())
                                    .AppendChild(new TextNode("aaa")).ParentNode.ParentNode.ParentNode.ParentNode.ParentNode
                    .AppendChild(new HTMLElement() { NodeName = "b" })
                        .AppendChild(new TextNode("ccc")).ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }

        [TestMethod]
        public void TreeMisnestedTagsHeisenbergNoFurthest()
        {
            //8.2.8.1 Misnested tags: <b><i></b></i>
            var doc = DocumentBuilder.Html(@"<p>1<b>2<i>3</b>4</i>5</p>");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new HTMLParagraphElement())
                        .AppendChild(new TextNode("1")).ParentNode
                        .AppendChild(new HTMLElement() { NodeName = "b" })
                            .AppendChild(new TextNode("2")).ParentNode
                            .AppendChild(new HTMLElement() { NodeName = "i" })
                                .AppendChild(new TextNode("3")).ParentNode.ParentNode.ParentNode
                        .AppendChild(new HTMLElement() { NodeName = "i" })
                            .AppendChild(new TextNode("4")).ParentNode.ParentNode
                        .AppendChild(new TextNode("5")).ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }

        [TestMethod]
        public void TreeMisnestedTagsHeisenbergWithFurthest()
        {
            //8.2.8.2 Misnested tags: <b><p></b></p>
            var doc = DocumentBuilder.Html(@"<b>1<p>2</b>3</p>");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new HTMLElement() { NodeName = "b" })
                        .AppendChild(new TextNode("1")).ParentNode.ParentNode
                    .AppendChild(new HTMLParagraphElement())
                        .AppendChild(new HTMLElement() { NodeName = "b" })
                            .AppendChild(new TextNode("2")).ParentNode.ParentNode
                        .AppendChild(new TextNode("3")).ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }

        [TestMethod]
        public void TreeUnclosedFormattingElements()
        {
            //8.2.8.6 Unclosed formatting elements
            var doc = DocumentBuilder.Html(@"<!DOCTYPE html>
<p><b class=x><b class=x><b><b class=x><b class=x><b>X<p>X<p><b><b class=x><b>X<p></b></b></b></b></b></b>X");

            var tree = new HTMLHtmlElement()
                .AppendChild(new HTMLHeadElement()).ParentNode
                .AppendChild(new HTMLBodyElement())
                    .AppendChild(new HTMLParagraphElement())
                        .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                            .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                .AppendChild(new HTMLElement() { NodeName = "b" })
                                    .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                        .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                            .AppendChild(new HTMLElement() { NodeName = "b" })
                                                .AppendChild(new TextNode("X")).ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode
                    .AppendChild(new HTMLParagraphElement())
                        .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                            .AppendChild(new HTMLElement() { NodeName = "b" })
                                .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                    .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                        .AppendChild(new HTMLElement() { NodeName = "b" })
                                            .AppendChild(new TextNode("X")).ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode
                    .AppendChild(new HTMLParagraphElement())
                        .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                            .AppendChild(new HTMLElement() { NodeName = "b" })
                                .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                    .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                        .AppendChild(new HTMLElement() { NodeName = "b" })
                                            .AppendChild(new HTMLElement() { NodeName = "b" })
                                                .AppendChild(new HTMLElement() { NodeName = "b" }.SetAttribute("class", "x"))
                                                    .AppendChild(new HTMLElement() { NodeName = "b" })
                                                        .AppendChild(new TextNode("X")).ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode
                    .AppendChild(new HTMLParagraphElement())
                        .AppendChild(new TextNode("X")).ParentNode.ParentNode.ParentNode;

            Assert.AreEqual(tree.ToHtml(), doc.DocumentElement.ToHtml());
        }
    }
}
