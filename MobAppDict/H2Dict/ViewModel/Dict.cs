using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using H2Dict.Model;
using H2Dict.Helper;

namespace H2Dict.ViewModel
{
    public class Dict
    {
        private ListWords _lstWord;
        private List<string> _lstTranslatedWords;
        private List<string> _lstFavoriteWords;
        public Dict()
        {
            _lstWord = new ListWords();
            _lstTranslatedWords = new List<string>();
            _lstFavoriteWords = new List<string>();

        }

        public ListWords LstWord
        {
            get { return _lstWord; }
        }

        public List<string> LstTranslatedWords
        {
            get { return _lstTranslatedWords; }
            set { _lstTranslatedWords = value; }
        }

        public List<string> LstFavoriteWords
        {
            get { return _lstFavoriteWords; }
            set { _lstFavoriteWords = value; }
        }

        // Method
        // Load All Words
        public async Task LoadListWords()
        {
            if (_lstWord.LstKey.Count != 0)
            {
                _lstWord.LstKey.Clear();
                _lstWord.LstLength.Clear();
                _lstWord.LstOffset.Clear();
            }
            _lstWord = await DataHelper.LoadListWords();
        }

        #region Search
        public async Task<string> Search(string key)
        {
            string res = null;
            key = key.ToLower();
            int ind = _lstWord.LstKey.FindIndex(x => x.Equals(key));
            if (ind >= 0)
            {
                int offset = GetDemicalValue(_lstWord.LstOffset[ind]);
                int length = GetDemicalValue(_lstWord.LstLength[ind]);
                res = await GetMeaning(offset, length);
                //res = offset + length + "";
                //res = _lstWord.LstOffset[ind] + " " + _lstWord.LstLength[ind] + " " + res;
            }

            if (res == null)
                res = "N/A";

            return res;
        }
        // Get list suggestion
        public List<string> GetSuggestion(string key)
        {
            key = key.ToLower();
            List<string> lstString = new List<string>();
            int len = key.Length;

            if (len < 3)
                return lstString;

            foreach (string word in _lstWord.LstKey)
            {
                if (word.Length >= len && word.Substring(0, len).Equals(key))
                {
                    int ind = _lstWord.LstKey.FindIndex(x => x.Equals(word));
                    for (int i = 0; i < 4; i++)
                    {
                        lstString.Add(_lstWord.LstKey[ind + i]);
                    }
                }
            }

            return lstString;
        }

        private int GetDemicalValue(string str)
        {
            String base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            int decValue = 0;
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                int pos = base64.IndexOf(str[i]);
                decValue += (int)Math.Pow(64, len - i - 1) * pos;
            }
            return decValue;
        }

        private async Task<string> GetMeaning (int offset, int length)
        {
            return await DataHelper.GetMeaning(offset, length);
        }
        #endregion

        #region Translated Words
        public async Task<List<string>>  LoadTranslatedWords()
        {
            _lstTranslatedWords = await DataHelperTranslatedWords.LoadListWords();
            return _lstTranslatedWords;
        }

        public async void UpdateTranslatedWords(string word)
        {
            word = word.ToLower();
            if (_lstTranslatedWords.Count == 0)
            {
                _lstTranslatedWords = await DataHelperTranslatedWords.LoadListWords();
            }

            int ind = _lstTranslatedWords.FindIndex(x => x.Equals(word));
            // Word not found
            if (ind == -1)
            {
                _lstTranslatedWords.Insert(0, word);

                if (_lstTranslatedWords.Count > 10)
                {
                    _lstTranslatedWords.RemoveAt(11);
                }
            }
            // Have been word
            else
            {
                _lstTranslatedWords.RemoveAt(ind);
                _lstTranslatedWords.Insert(0,word);
            }

            await DataHelperTranslatedWords.SaveListWords(_lstTranslatedWords);

            //_lstTranslatedWords.Clear();
        }

        public async Task ClearTranslatedWords()
        {
            await DataHelperTranslatedWords.SaveListWords(_lstTranslatedWords);

            _lstTranslatedWords.Clear();
        }

        #endregion

        #region Favorite Words

        public async Task<List<string>>  LoadFavoriteWords()
        {
            _lstFavoriteWords = await DataHelperFavoriteWords.LoadListWords();
            return _lstFavoriteWords;
        }

        public async Task UpdateFavoriteWords()
        {
            await DataHelperFavoriteWords.SaveListWords(_lstFavoriteWords);
        }

        public async void UpdateFavoriteWords(string word)
        {
            word = word.ToLower();
            if (_lstTranslatedWords.Count == 0)
            {
                _lstTranslatedWords = await DataHelperFavoriteWords.LoadListWords();
            }

            int ind = _lstFavoriteWords.FindIndex(x => x.Equals(word));
            // Word not found
            if (ind == -1)
            {
                _lstFavoriteWords.Insert(0, word);

//                if (_lstFavoriteWords.Count > 10)
//                {
//                    _lstFavoriteWords.RemoveAt(11);
//                }
            }
            // Have been word
            else
            {
                _lstFavoriteWords.RemoveAt(ind);
                _lstFavoriteWords.Insert(0, word);
            }

            await DataHelperFavoriteWords.SaveListWords(_lstFavoriteWords);

            //_lstTranslatedWords.Clear();
        }

        #endregion
    }
}
