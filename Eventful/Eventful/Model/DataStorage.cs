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

        public static Event LoadEventFromDisk(string filename)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Event));
                TextReader textReader = new StreamReader(filename);
                Event ev;
                ev = (Event)deserializer.Deserialize(textReader);
                textReader.Close();
                ev.IsChanged = false;
                return ev;
            }
            catch
            {
                return null;
            }
        }
        public static Deck LoadDeckFromDisk(string deckTitle)
        {
            string fullpath = String.Concat(StorageDirectory, deckTitle);
            if (!Directory.Exists(fullpath)) return null;
            if (deckTitle.Length <= 0) return null;
            if (deckTitle[0] == '.') return null;
            Deck newDeck = new Deck(deckTitle);
            foreach (string file in Directory.EnumerateFiles(fullpath, "*.event"))
            {
                Event newEvent = LoadEventFromDisk(file);
                if (newEvent != null)
                    newDeck.Events.Add(newEvent);
            }
            return newDeck;
        }
        public static List<Deck> LoadAllDecksFromDisk()
        {
            List<Deck> decks = new List<Deck>();
            try
            {
                foreach (string directory in Directory.EnumerateDirectories(StorageDirectory))
                {
                    string subDirectory = directory.Replace(StorageDirectory, "");
                    Deck deck = LoadDeckFromDisk(subDirectory);
                    if (deck != null)
                        decks.Add(deck);
                }
                return decks;
            }
            catch
            {
                return new List<Deck>();
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
