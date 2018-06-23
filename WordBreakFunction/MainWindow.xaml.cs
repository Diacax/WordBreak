using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordBreakFunction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string vowels = "aeiouyAEIOUY";
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        public MainWindow()
        {
            InitializeComponent();
            dict.Add("word", "Apple");
            dict.Add("AnotherWord", "Pie");
            dict.Add("ThirdWord", "Disdain");
            dict.Add("FourthWord", "Pumpkin");

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Do we need to test if the string contains any other chars such as *#*$(% or numbers?
            // Check to see if the string contains any vowels in English including Y so we know if we have any words. 
            string stringToBreak = txtWordList.Text;
            string foundWords = null; 
            bool containsVowel = CheckForVowels(stringToBreak);
            string substring = null;
            Int32 substringstart = 0;
            try
            {
                txtblBrokenWords.Text = ""; 
                if (containsVowel)
                {
                    if (dict.ContainsValue(stringToBreak))
                    {
                        foundWords += stringToBreak; 
                        
                    }
                    else
                    {
                        for (int i = 0; i <= stringToBreak.Length; i++)
                        {
                            if (substringstart != 0)
                                substring = stringToBreak.Substring(substringstart, i - substringstart);
                            else
                                substring = stringToBreak.Substring(substringstart, i);

                            if (dict.ContainsValue(substring))
                            {
                                if (foundWords == "")
                                    foundWords += substring;
                                else
                                    foundWords += " " + substring;

                                substringstart = i; 
                            }
                        } 
                    }
                    txtblBrokenWords.Text = foundWords;  
                }
                else
                {
                    txtblBrokenWords.Text = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public bool inDictionary(string word)
        {
            if (dict.ContainsValue(word))
                return true;
            else
                return false;
        }


        public bool CheckForVowels(string wordList)
        {
            foreach (char vowel in vowels)
            {
                if (wordList.Contains(vowel))
                {
                    return true;
                }
            }
            return false;
        }
        public string WebCall(string url, string jsonContent, string method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            if (method == "POST")
            {
                Byte[] byteArray = encoding.GetBytes(jsonContent);
                
                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            else if (method == "GET")
            {
                request.Headers.Add("X-Mashape-Key", "U9SRM1TJlfmshXTUjBw2N329Kv7hp17WT3LjsnPb6l4Zj7E4HC");
                request.ContentType = @"application/json";
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(receiveStream, encode);

                    // Dumps the 256 characters on a string and dumps to variable for exposure to be deserialized.
                    return readStream.ReadToEnd();

                }
            }
            catch (WebException ex)
            {
                return ex.ToString();
                // Log exception and throw as for GET example above
            }
        }
        
        public void populateTxtFromAPI(string stringToBreak, int strLength)
        {
            // This can be used in the event that we need to use an api to another system instead of a dictionary. 
            int startOfString = 0;
            txtblBrokenWords.Text = "";
            for (int i = 0; i < strLength; i++)
            {
                string substring = stringToBreak.Substring(startOfString, i);
                string url = "https://wordsapiv1.p.mashape.com/words/" + substring;
                string requestReturn = WebCall(url, "", "GET");

                // Advanced API call to test if words are in the Dictionary and return. 
                // Make Call to word api to verify if this section is a word. 
                if (!requestReturn.Contains("404"))
                {
                  txtblBrokenWords.Text += " " + substring;
                }

            }
        }

        private void txtWordList_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
