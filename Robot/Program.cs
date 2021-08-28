using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Threading;

namespace Robot
{
    class Program
    {
        private static char Try;

        private static char[] KeyboardArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                                      'A', 'B', 'C', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
                                      'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                                      'V', 'W', 'X', 'Y', 'Z'};

        private static string SSML_START_CHAR = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"Microsoft Zira Desktop\"><say-as interpret-as=\"characters\">";
        private static string SSML_END_CHAR = "</say-as></voice></speak>";

        private static string SSML_START_STR = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"Microsoft Zira Desktop\">"; // <mstts:silence type=\"Leading\" value=\"100ms\">
        private static string SSML_END_STR = "</voice></speak>";

        private static string SSML_PAUSE = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\"><voice name=\"Microsoft Zira Desktop\"><break time=\"100ms\" /> </voice></speak>";

        private static SpeechSynthesizer Synthesizer;

        static void Main(string[] args)
        {
            Synthesizer = new SpeechSynthesizer();
            Synthesizer.SetOutputToDefaultAudioDevice();
            Synthesizer.Rate = 0;

            int loops = 100;

            if (args.Length > 0)
            {
                Int32.TryParse(args[0], out loops);
            }

            Console.WriteLine("Press any button to begin...");
            Console.ReadLine();

            //SpeakString("Hello, Moany. It's me, your friend, the letter robot.");
            Synthesizer.Rate = 3;
           // SpeakString("  Beep bop boop, beep beep");
            Synthesizer.Rate = 0;
            //SpeakString("  I am happy to see you. I like your vacation house. It must have been fun to go out on the big raft.");
            //SpeakString("  Let's play a little game. I will say a letter, and you find it and press it!");

            int aCurrentLoop = 0;

            while (aCurrentLoop < loops)
            {
                Thread.Sleep(250);
                   
                GiveCharactersToType();
                aCurrentLoop++;

                if (aCurrentLoop >= loops)
                {
                    SpeakString("Ok Moany, I am very sleepy. I must stop and go to sleep for a little bit. Night Night");

                    break;
                }
                SpeakString("Ok Moany, let's do another one.");
            }


            //while (true)
            //{
            //    ConsoleKeyInfo aKey = Console.ReadKey(true);
            //    Console.WriteLine(aKey.KeyChar);
            //}

        }

        private static void SpeakCharacter(char pChar)
        {
            Synthesizer.SpeakSsml(String.Format("{0}", SSML_PAUSE));
            Synthesizer.SpeakSsml(String.Format("{0}{1}{2}", SSML_START_CHAR, pChar, SSML_END_CHAR));
        }

        private static void SpeakString(string pStr)
        {
            Synthesizer.SpeakSsml(String.Format("{0}{1}{2}", SSML_START_STR, pStr, SSML_END_STR));
        }


        private static void GiveCharactersToType()
        {
            Random rand = new Random();
            int aKeyboardIndex = rand.Next(35);
            char aCharToType = KeyboardArray[aKeyboardIndex];
            bool isFound = false;

            if (aKeyboardIndex < 10)
            {
                SpeakString("<break time=\"100ms\" />Can you find the number " + aCharToType + "?");
            }
            else
            {
                SpeakString("<break time=\"100ms\" />Can you find the letter " + aCharToType + "?");
            }

            while (!isFound)
            {
                try
                {
                    //char aTry = ' ';

                    ThreadStart aGetTryRef = new ThreadStart(WaitForKey);
                    Thread aGetTryThread = new Thread(aGetTryRef);
                    Console.WriteLine("Getting Try");
                    aGetTryThread.Start();
                    //

                    aGetTryThread.Join();
                    Console.WriteLine("Dont getting try");

                    if (String.Compare(Try.ToString(), aCharToType.ToString(), true) == 0)
                    {
                        SpeakCharacter(Try);

                        SpeakString("Nice work Moany! You found it!");

                        isFound = true;                        
                    }
                    else
                    {
                        SpeakCharacter(Try);
                        SpeakString("No Silly! We are looking for: ");
                        SpeakCharacter(aCharToType);
                    }
                }
                catch (FormatException)
                {
                    SpeakString("No Silly! That's not even a letter or number!");
                    SpeakString("We are looking for ");
                    SpeakCharacter(aCharToType);
                }
                Thread.Sleep(1500);
                
            }
        }

        private static void WaitForKey()
        {
            Try = Console.ReadKey(true).KeyChar;
        }

        private static void GiveLettersToType()
        {
            Random rand = new Random();
            int aKeyboardIndex = rand.Next(36);
            char aCharToType = KeyboardArray[aKeyboardIndex];
            bool isFound = false;


            if (aKeyboardIndex < 10)
            {
                Synthesizer.SpeakAsync("Can you find the number " + aCharToType + "?");
            }
            else
            {
                Synthesizer.SpeakAsync("Can you find the letter " + aCharToType + "?");
            }

            while (!isFound)
            {
                char aTry = ' ';

                while (Console.KeyAvailable)
                {
                    aTry = Console.ReadKey(true).KeyChar;
                }
               

                Console.WriteLine(aTry);
                if (String.Compare(aTry.ToString(), aCharToType.ToString(), true) == 0)
                {
                    Synthesizer.SpeakAsync("Nice work, Moany! You found it!");

                    isFound = true;

                    Synthesizer.SpeakAsync("Ok, Moany, let's do another one.");
                }
                else
                {
                    Synthesizer.SpeakAsync("No, Silly! You found" + aTry + ". We are looking for " + aCharToType + ". Please try again.");
                }

            }
        }
    }
}
