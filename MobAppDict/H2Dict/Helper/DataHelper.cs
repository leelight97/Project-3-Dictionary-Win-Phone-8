using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using H2Dict.Model;

namespace H2Dict.Helper
{
    public class DataHelper
    {
        private const string fileInd = "index.txt";
        private const string fileDict = "dict.txt";

        private static DataHelper _dataHelper = new DataHelper();
        private ListWords _lstWords;

        public static async Task<ListWords> LoadListWords()
        {
            return await _dataHelper.LoadListWordsAsync();
        }

        private async Task<ListWords> LoadListWordsAsync()
        {
            if(_lstWords == null)
                _lstWords = new ListWords();

            if (_lstWords.LstKey.Count != 0 && !App.ChangeDict)
                return _lstWords;

            string result = null;
            string path = @"ms-appx:///Data/" + App.TypeDictIns.GetTypeDict() + "/" + fileInd;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
            
            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
                result = await sRead.ReadToEndAsync();
            
            string[] lines = result.Split(new char[1] { '\n' });

            foreach(string line in lines)
            {
                string[] strs = line.Split(new char[1] { '\t' });

                _lstWords.LstKey.Add(strs[0]);
                _lstWords.LstOffset.Add(strs[1]);
                _lstWords.LstLength.Add(strs[2]);
            }
            App.ChangeDict = false;
            return _lstWords;
        }

        public static async Task<string> GetMeaning(int offset, int length)
        {
            return await _dataHelper.ReadFile(offset, length);
        }

        private async Task<string> ReadFile(int offset, int length)
        {
            var fold = Windows.Storage.ApplicationData.Current.LocalFolder;

            string result = null;
            byte[] buff = new byte[length];

            string path = @"ms-appx:///Data/" + App.TypeDictIns.GetTypeDict() + "/" + fileDict;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
            int pos = 0;
            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync() ))
            {
                //result = await sRead.ReadToEndAsync();
                sRead.BaseStream.Seek(offset, SeekOrigin.Begin);
                pos = await sRead.BaseStream.ReadAsync(buff, 0, length);
                

            }
            result = Encoding.UTF8.GetString(buff,0,length);

            return result;
        }

    }
}
