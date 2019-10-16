using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimerConsumerLib;
using MimerConsumerLib.DTOs;
using System.Collections.Generic;

namespace MimerConsumerUnitTestProject
{
    [TestClass]
    public class MimerConsumerUnitTests
    {
        [TestMethod]
        public void Test10LatestArticles()
        {
            MimerConsumer mc = new MimerConsumer();
            List<Article> articles = mc.GetNLatestArticles(10);
            Assert.AreEqual(10, articles.Count, "Did not get the expected count of articles (10)");
        }

        [TestMethod]
        public void TestAllLatestArticles()
        {
            MimerConsumer mc = new MimerConsumer();
            List<Article> articles = mc.GetLatestArticles();
            Assert.AreEqual(20, articles.Count, "Did not get the expected count of articles (20)");
        }

        [TestMethod]
        public void TestZeroLatestArticles()
        {
            MimerConsumer mc = new MimerConsumer();
            List<Article> articles = mc.GetNLatestArticles(0);
            Assert.AreEqual(0, articles.Count, "Did not get the expected count of articles (0)");
        }

        [TestMethod]
        public void TestSites()
        {
            MimerConsumer mc = new MimerConsumer();
            List<Site> sites = mc.GetSites();
            Assert.IsTrue(sites.Count != 0, "No sites found");
        }

        [TestMethod]
        public void TestFrontPageEditorSites()
        {
            MimerConsumer mc = new MimerConsumer();
            List<Site> sites = mc.GetFrontPageEditorsSites();
            Assert.IsTrue(sites.Count != 0, "No front page editor sites found");
        }
    }
}
