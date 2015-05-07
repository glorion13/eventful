using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eventful.Model
{
    public static class DataStorage
    {
        public static string DefaultStorageDirectory = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Eventful\");
        public static void SaveEventToXml(Event ev, Deck deck, string StorageDirectory)
        {
            try
            {
                SaveEventToXmlUnsafe(ev, deck, StorageDirectory);
            }
            catch (DirectoryNotFoundException)
            {
                CreateStorageDirectory(StorageDirectory);
                CreateStorageDirectory(String.Concat(StorageDirectory, PrepareFilename(deck.Title)));
                SaveEventToXml(ev, deck, StorageDirectory);
            }
            catch
            {

            }
        }

        private static void SaveEventToXmlUnsafe(Event ev, Deck deck, string StorageDirectory)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Event));
            TextWriter textWriter = new StreamWriter(String.Concat(StorageDirectory, PrepareFilename(deck.Title), @"\", PrepareFilename(ev.Title), ".xml"));
            serializer.Serialize(textWriter, ev);
            textWriter.Close();
        }

        private static string PrepareFilename(string text)
        {
            string[] restrictedStrings  = new string[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            string replacementString = "";
            foreach (string restrictedString in restrictedStrings)
                text = text.Replace(restrictedString, replacementString);
            return text;
        }

        private static void CreateStorageDirectory(string directory)
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch
            {

            }
        }

        public static Event LoadEventFromXml(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Event));
            TextReader textReader = new StreamReader(filename);
            Event ev;
            ev = (Event)deserializer.Deserialize(textReader);
            textReader.Close();
            ev.IsChanged = false;
            return ev;
        }


        public static void DeleteEvent(Event ev, Deck deck, string StorageDirectory)
        {
            BackupEvent(ev, deck, StorageDirectory);
        }
        public static void DeleteDeck(Deck deck, string StorageDirectory)
        {
            BackupDeck(deck, StorageDirectory);
        }

        private static void BackupEvent(Event ev, Deck deck, string StorageDirectory)
        {
            try
            {
                CreateStorageDirectory(String.Concat(StorageDirectory, PrepareFilename(deck.Title), @"\", ".Backups"));
                Directory.Move(String.Concat(StorageDirectory, PrepareFilename(deck.Title), @"\", PrepareFilename(ev.Title), ".xml"), String.Concat(StorageDirectory, PrepareFilename(deck.Title), @"\", @".Backups\", PrepareFilename(DateTime.Now.ToString()), ".xml"));
            }
            catch
            {
            }
        }

        private static void BackupDeck(Deck deck, string StorageDirectory)
        {
            try
            {
                CreateStorageDirectory(String.Concat(StorageDirectory, ".Backups"));
                Directory.Move(String.Concat(StorageDirectory, PrepareFilename(deck.Title), @"\"), String.Concat(StorageDirectory, @".Backups\", PrepareFilename(DateTime.Now.ToString())));
            }
            catch
            {
            }
        }

    }
}
