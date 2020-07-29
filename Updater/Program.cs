using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Tweetinvi;

namespace Updater
{
    class Program
    {
        public const string TWEET_ANCHOR = "<!-- BEGIN TWEETS -->";
        public const string END_TWEET_ANCHOR = "<!-- END TWEETS -->";

        public const int MAX_TWEETS = 7;

        static int Main(string[] args)
        {
            var consumerKey = args[0];
            var consumerSecret = args[1];
            var accessToken = args[2];
            var accessTokenSecret = args[3];

            var credentials = Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);

            var user = User.GetAuthenticatedUser(credentials);
            var readmeText = File.ReadAllText("README.md", Encoding.UTF8);

            var parameter = Timeline.CreateUserTimelineParameter();
            parameter.IncludeRTS = false;
            parameter.ExcludeReplies = true;
            parameter.MaximumNumberOfTweetsToRetrieve = 100;

            var timeline = Timeline.GetUserTimeline(user.Id, parameter);

            var strTweets = new StringBuilder();

            var tweetsStart = readmeText.IndexOf(TWEET_ANCHOR) + TWEET_ANCHOR.Length;
            var tweetsEnd = readmeText.IndexOf(END_TWEET_ANCHOR);

            strTweets.Append(readmeText[0..tweetsStart]);

            var tweetCount = 0;

            foreach(var item in timeline)
            {
                var tweetText = item.Text.Replace("\n", "");
                tweetText = Regex.Replace(tweetText, @"https://t\.co/[a-zA-Z0-9]+", "");

                strTweets.AppendLine()
                    .AppendFormat("#### [{0}]({1})", tweetText, item.Url)
                    .AppendLine();

                tweetCount++;

                if(tweetCount >= MAX_TWEETS)
                    break;
            }

            strTweets.Append(readmeText[tweetsEnd..]);

            Console.WriteLine(strTweets.ToString());
            return 0;
        }
    }
}
