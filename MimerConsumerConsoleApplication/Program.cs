using MimerConsumerConsoleApplication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimerConsumerLib;
using MimerConsumerLib.DTOs;

namespace MimerConsumerConsoleApplication
{
    class Program
    {
        private static readonly MimerConsumer Consumer;

        static Program()
        {
			Consumer = new MimerConsumer();
        }

        private static void GetAndPrint10LatestArticles()
        {
            Log.Write("Getting 10 latest articles...");
            List<Article> tenLatestArticles = Consumer.GetNLatestArticles(10);
            foreach (var a in tenLatestArticles.Select((value, index) => new { value, index }))
            {
                Log.Write(String.Format("Article {0}: \"{1}\"", a.index + 1, a.value.title));
            }
        }

        private static void GetAndPrintFrontPageEditorsSites()
        {
            Log.Write("Getting front page editors sites...");
            List<Site> frontPageSites = Consumer.GetFrontPageEditorsSites();
            if (!frontPageSites.Any())
                Log.Write("data mangler");
            else
            {
                foreach (var s in frontPageSites.Select((value, index) => new { value, index }))
                {
                    Log.Write(String.Format("Site {0}: \"{1}\"", s.index + 1, (s.value as Site).GetTitle()));
                }
            }
        }

        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Log.Write("MimerConsumer starting...");

			GetAndPrint10LatestArticles();

            GetAndPrintFrontPageEditorsSites();

			Log.Write("MimerConsumer finished. Press ENTER to terminate.");

			Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.WriteException(Log.Level.Critical, String.Format("Unhandled exception"), (Exception)e.ExceptionObject);
            Console.ReadLine();
            Environment.Exit(-1);
        }
    }
}
