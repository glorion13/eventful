using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static string StorageDirectory = DefaultStorageDirectory;

        public static bool SaveEventToDisk(Event ev, Deck deck)
        {
            if (deck == null || ev == null || StorageDirectory == null) return false;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Event));
                TextWriter textWriter = new StreamWriter(String.Concat(StorageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", RemoveIllegalCharactersFromFilename(ev.Title), " #", ev.Id, ".event"));
                serializer.Serialize(textWriter, ev);
                textWriter.Close();
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    SaveDeckToDisk(deck);
                    SaveEventToDisk(ev, deck);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool SaveDeckToDisk(Deck deck)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(StorageDirectory, RemoveIllegalCharactersFromFilename(deck.Title)));
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    Directory.CreateDirectory(StorageDirectory);
                    SaveDeckToDisk(deck);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static Event LoadEventFromDisk(string title)
        {
            string fullpath = String.Concat(StorageDirectory, title);
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Event));
                TextReader textReader = new StreamReader(fullpath);
                Event ev = new Event();
                ev = (Event)deserializer.Deserialize(textReader);
                textReader.Close();
                return ev;
            }
            catch
            {
                return null;
            }
        }
        public static async void LoadDeckEventsFromDisk(Deck deck)
        {
            string fullpath = String.Concat(StorageDirectory, deck.Title);
            deck.Events.Clear();
            foreach (string file in await Task.Run(() => Directory.EnumerateFiles(fullpath, "*.event")))
            {
                string subDirectory = file.Replace(DataStorage.StorageDirectory, "");
                Event newEvent = await Task.Run(() => LoadEventFromDisk(subDirectory));
                newEvent.IsChanged = false;
                if (newEvent != null)
                    deck.Events.Add(newEvent);
            }
        }
        public static Deck LoadDeckMappingFromDisk(string deckMapping)
        {
            string fullpath = String.Concat(StorageDirectory, deckMapping);
            if (!Directory.Exists(fullpath)) return null;
            if (deckMapping.Length <= 0) return null;
            if (deckMapping[0] == '.') return null;
            return new Deck(deckMapping);
        }
        public static async void LoadAllDeckMappingsFromDisk(ObservableCollection<Deck> decks)
        {
            decks.Clear();
            foreach (string folder in await Task.Run(() => Directory.EnumerateDirectories(StorageDirectory)))
            {
                string subDirectory = folder.Replace(DataStorage.StorageDirectory, "");
                Deck deck = DataStorage.LoadDeckMappingFromDisk(subDirectory);
                if (deck != null)
                    decks.Add(deck);
            }
        }

        public static void DeleteEvent(Event ev, Deck deck)
        {
            BackupEvent(ev, deck);
        }
        public static void DeleteDeck(Deck deck)
        {
            BackupDeck(deck);
        }

        private static void BackupEvent(Event ev, Deck deck)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(StorageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", ".Backups"));
                Directory.Move(String.Concat(StorageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", RemoveIllegalCharactersFromFilename(ev.Title), " #", ev.Id, ".event"), String.Concat(StorageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", @".Backups\", String.Concat(RemoveIllegalCharactersFromFilename(ev.Title), " #", ev.Id, " "), RemoveIllegalCharactersFromFilename(DateTime.Now.ToString()), ".event"));
            }
            catch
            {
            }
        }
        private static void BackupDeck(Deck deck)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(StorageDirectory, ".Backups"));
                Directory.Move(String.Concat(StorageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\"), String.Concat(StorageDirectory, @".Backups\", RemoveIllegalCharactersFromFilename(deck.Title), " ", RemoveIllegalCharactersFromFilename(DateTime.Now.ToString())));
            }
            catch
            {
            }
        }

        public static void RenameDeck(Deck deck, string newDeckTitle)
        {
            try
            {
                Directory.Move(
                    String.Concat(StorageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\"),
                    String.Concat(StorageDirectory, newDeckTitle, @"\")
                );
            }
            catch
            {
            }
        }

        private static string RemoveIllegalCharactersFromFilename(string filename)
        {
            string[] restrictedStrings = new string[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            foreach (string restrictedString in restrictedStrings)
                filename = filename.Replace(restrictedString, "");
            return filename;
        }
    }
}
