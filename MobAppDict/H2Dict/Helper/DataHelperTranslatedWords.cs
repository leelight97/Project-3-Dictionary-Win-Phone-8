using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace H2Dict.Helper
{
    public class DataHelperTranslatedWords
    {
        private const string FileName = "TranslatedWords.txt";
        private string _typeDict;
        private static DataHelperTranslatedWords _dataHelper = new DataHelperTranslatedWords();
        private List<string> _lstWords;

        #region Load from file
        public async static Task<List<string>> LoadListWords()
        {
            return await _dataHelper.LoadListWordsAsync();
        }

        private async Task<List<string>> LoadListWordsAsync()
        {
            _lstWords = new List<string>();

            if (_lstWords.Count != 0 && !App.ChangeDict)
                return _lstWords;

           
            _typeDict = App.TypeDictIns.GetTypeDict();

            string result = null;
            StorageFolder local = ApplicationData.Current.LocalFolder;
            StorageFolder fold;
            if (await FolderExists(local, _typeDict))
            {
                fold = await local.GetFolderAsync(_typeDict);
            }
            else
            {
                fold = await local.CreateFolderAsync(_typeDict);
            }

            if (!await FileExists(fold, FileName))
                fold.CreateFileAsync(FileName);

            StorageFile file = await fold.GetFileAsync(FileName);

            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
                result = await sRead.ReadToEndAsync();

            string[] lines = result.Split(new char[2] { '\r','\n' });
            
            foreach (string line in lines)
            {
                if(line != "")
                    _lstWords.Add(line);
            }

            return _lstWords;
        }

        public static async Task SaveListWords(List<string> lstWords)
        {
            string value = "";

            foreach (var words in lstWords)
            {
                value = value + words + "\r\n";
            }
            await _dataHelper.SaveListWords(value);
            
        }

        #endregion

        #region Save to file
        private async Task SaveListWords(string value)
        {
            _typeDict = App.TypeDictIns.GetTypeDict();

            StorageFolder local = ApplicationData.Current.LocalFolder;
            StorageFolder fold;
            if (await FolderExists(local, _typeDict))
            {
                fold = await local.GetFolderAsync(_typeDict);
            }
            else
            {
                fold = await local.CreateFolderAsync(_typeDict);
            }

            if (!await FileExists(fold, FileName))
                fold.CreateFileAsync(FileName);

            StorageFile file = await fold.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

            using (StreamWriter sWrite = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                await sWrite.WriteAsync(value);
                
            }
            _lstWords.Clear();
            _lstWords = await LoadListWords();
        }

        #endregion

        #region Support method
        private async Task<bool> FolderExists(StorageFolder folder, string name)
        {
            try
            {
                StorageFolder file = await folder.GetFolderAsync(name);
            }
            catch { return false; }
            return true;
        }

        private async Task<bool> FileExists(StorageFolder folder, string name)
        {
            try
            {
                StorageFile file = await folder.GetFileAsync(name);
            }
            catch { return false; }
            return true;
        }
        #endregion
    }
}
