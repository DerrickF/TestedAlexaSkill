﻿//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using AlexaSkill;
using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using NLog;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.Slu;
using AlexaSkillsKit.UI;
using Newtonsoft.Json;

namespace Sample.Controllers
{
    public class SampleSessionSpeechlet : Speechlet
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        // Note: NAME_KEY being a JSON property key gets camelCased during serialization
        private const string NAME_KEY = "name";
        private const string NAME_SLOT = "Name";

        /// <summary>
        /// return true if you want request to be processed, otherwise false
        /// </summary>
        public override bool OnRequestValidation(
            SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope)
        {
            return true;
        }


        public override void OnSessionStarted(SessionStartedRequest request, Session session)
        {
            Log.Info($"OnSessionStarted requestId={request.RequestId}, sessionId={session.SessionId}");
        }


        public override SpeechletResponse OnLaunch(LaunchRequest request, Session session)
        {
            Log.Info($"OnLaunch requestId={request.RequestId}, sessionId={session.SessionId}");
            return GetWelcomeResponse();
        }


        public override SpeechletResponse OnIntent(IntentRequest request, Session session)
        {
            Log.Info($"OnIntent requestId={request.RequestId}, sessionId={session.SessionId}");

            // Get intent from the request object.
            Intent intent = request.Intent;
            string intentName = (intent != null) ? intent.Name : null;

            // Note: If the session is started with an intent, no welcome message will be rendered;
            // rather, the intent specific response will be returned.
            if ("MyNameIsIntent".Equals(intentName))
            {
                return SetNameInSessionAndSayHello(intent, session);
            }
            else if ("WhatsMyNameIntent".Equals(intentName))
            {
                return GetNameFromSessionAndSayHello(intent, session);
            }
            else if ("HelloWorldIntent".Equals(intentName))
            {
                return HelloWorldResponse(intent,session);
            }
            else if ("PlayLatest".Equals(intentName))
            {
                return PlayLatestPodcast(intent);
            }
            else if ("ComplementIntent".Equals(intentName))
            {
                return ComplementResponse(intent, session);
            }
            else
            {
                throw new SpeechletException("Invalid Intent");
            }
        }

        private SpeechletResponse PlayLatestPodcast(Intent intent)
        {
            var feedUrl ="http://ajax.googleapis.com/ajax/services/feed/load?v=1.0&num=10&q=http://www.tested.com/podcast-xml/this-is-only-a-test/";

            var responseString = "";

            using (var client = new HttpClient())
            {
                responseString = client.GetStringAsync(feedUrl).Result;
            }
            PodcastFeed feed = JsonConvert.DeserializeObject<PodcastFeed>(responseString);
            var tmp = feed.ToString();

            string speechOutput = "";
            return BuildSpeechletResponse(intent.Name, speechOutput, false);
        }


        public override void OnSessionEnded(SessionEndedRequest request, Session session)
        {
            Log.Info($"OnSessionEnded requestId={request.RequestId}, sessionId={session.SessionId}");
        }


        /**
         * Creates and returns a {@code SpeechletResponse} with a welcome message.
         * 
         * @return SpeechletResponse spoken and visual welcome message
         */
        private SpeechletResponse GetWelcomeResponse()
        {
            // Create the welcome message.
            string speechOutput =
                "Welcome to the Alexa AppKit session sample app, please tell me your name by saying, my name is Sam";

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return BuildSpeechletResponse("Welcome", speechOutput, false);
        }


        /**
         * Creates a {@code SpeechletResponse} for the intent and stores the extracted name in the
         * Session.
         * 
         * @param intent
         *            intent for the request
         * @return SpeechletResponse spoken and visual response the given intent
         */
        private SpeechletResponse SetNameInSessionAndSayHello(Intent intent, Session session)
        {
            // Get the slots from the intent.
            Dictionary<string, Slot> slots = intent.Slots;

            // Get the name slot from the list slots.
            Slot nameSlot = slots[NAME_SLOT];
            string speechOutput = "";

            // Check for name and create output to user.
            if (nameSlot != null)
            {
                // Store the user's name in the Session and create response.
                string name = nameSlot.Value;
                session.Attributes[NAME_KEY] = name;
                speechOutput = String.Format(
                    "Hello {0}, now I can remember your name, you can ask me your name by saying, whats my name?", name);
            }
            else
            {
                // Render an error since we don't know what the users name is.
                speechOutput = "I'm not sure what your name is, please try again";
            }

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return BuildSpeechletResponse(intent.Name, speechOutput, false);
        }


        /**
         * Creates a {@code SpeechletResponse} for the intent and get the user's name from the Session.
         * 
         * @param intent
         *            intent for the request
         * @return SpeechletResponse spoken and visual response for the intent
         */
        private SpeechletResponse GetNameFromSessionAndSayHello(Intent intent, Session session)
        {
            string speechOutput = "";
            bool shouldEndSession = false;

            // Get the user's name from the session.
            string name = (String)session.Attributes[NAME_KEY];

            // Check to make sure user's name is set in the session.
            if (!String.IsNullOrEmpty(name))
            {
                speechOutput = String.Format("Your name is {0}, goodbye", name);
                shouldEndSession = true;
            }
            else
            {
                // Since the user's name is not set render an error message.
                speechOutput = "I'm not sure what your name is, you can say, my name is Sam";
            }

            return BuildSpeechletResponse(intent.Name, speechOutput, shouldEndSession);
        }

        /**
         * Creates a {@code SpeechletResponse} for the intent and get the user's name from the Session.
         * 
         * @param intent
         *            intent for the request
         * @return SpeechletResponse spoken and visual response for the intent
         */
        private SpeechletResponse HelloWorldResponse(Intent intent, Session session)
        {
           
            // Send speech output
            var speechOutput = String.Format("Hello World");

            return BuildSpeechletResponse(intent.Name, speechOutput, true);
        }

        /**
         * Creates a {@code SpeechletResponse} for the intent and get the user's name from the Session.
         * 
         * @param intent
         *            intent for the request
         * @return SpeechletResponse spoken and visual response for the intent
         */
        private SpeechletResponse ComplementResponse(Intent intent, Session session)
        {

            // Get the slots from the intent.
            Dictionary<string, Slot> slots = intent.Slots;

            // Get the name slot from the list slots.
            Slot nameSlot = slots["FirstName"];
            string speechOutput = "";
            string name = "";

            // Check for name and create output to user.
            if (nameSlot != null)
            {
                // Store the user's name in the Session and create response.
                name = nameSlot.Value;
            }

            
            var complements = new List<string>
            {
                "Your perspective on life is refreshing.",
                "You have a great sense of humor.",
                "On a scale from 1 to 10, you're an 1.1",
                "Your ability to recall random factoids at just the right time is impressive.",
                "Your bellybutton is kind of adorable.",
                "Being around you is like being on a happy little vacation.",
                "Even I can see how hot you are, and I dont even have eyes.",
                "I wish I had hands, so I could give you a high five."

            };
            // Get random complement
            var random = new Random();
            int index = random.Next(complements.Count);
            // Send speech output
            speechOutput = name + " " + complements[index];

            return BuildSpeechletResponse(intent.Name, speechOutput, true);
        }


        /**
         * Creates and returns the visual and spoken response with shouldEndSession flag
         * 
         * @param title
         *            title for the companion application home card
         * @param output
         *            output content for speech and companion application home card
         * @param shouldEndSession
         *            should the session be closed
         * @return SpeechletResponse spoken and visual response for the given input
         */
        private SpeechletResponse BuildSpeechletResponse(string title, string output, bool shouldEndSession)
        {
            // Create the Simple card content.
            SimpleCard card = new SimpleCard();
            card.Title = String.Format("SessionSpeechlet - {0}", title);
            card.Content = String.Format("SessionSpeechlet - {0}", output);

            // Create the plain text output.
            PlainTextOutputSpeech speech = new PlainTextOutputSpeech();
            speech.Text = output;

            // Create the speechlet response.
            SpeechletResponse response = new SpeechletResponse();
            response.ShouldEndSession = shouldEndSession;
            response.OutputSpeech = speech;
            response.Card = card;
            return response;
        }
    }
}