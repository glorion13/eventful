using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eventful.Model
{
    public static class DataStorage
    {
        public static string DefaultStorageDirectory = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Eventful\");
        public static string StorageDirectory = DefaultStorageDirectory;

        private static string deckDirectoryName = "decks\\";
        private static string tagDirectoryName = "tags\\";
        private static string variableDirectoryName = "variables\\";

        private static bool CreateInitialDirectories()
        {
            try
            {
                Directory.CreateDirectory(StorageDirectory);
                Directory.CreateDirectory(String.Concat(StorageDirectory, deckDirectoryName));
                Directory.CreateDirectory(String.Concat(StorageDirectory, tagDirectoryName));
                Directory.CreateDirectory(String.Concat(StorageDirectory, variableDirectoryName));
                return true;
            }
            catch
            {
                System.Windows.MessageBox.Show("Problem reading/writing to disk. There is a chance your data will not be saved.");
                return false;
            }
        }

        public static bool SaveEventToDisk(Event ev, Deck deck)
        {
            if (deck == null || ev == null || StorageDirectory == null) return false;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Event));
                TextWriter textWriter = new StreamWriter(String.Concat(
                    StorageDirectory,
                    deckDirectoryName,
                    RemoveIllegalCharactersFromFilename(deck.Title),
                    @"\", RemoveIllegalCharactersFromFilename(ev.Title),
                    " #",
                    ev.Id,
                    ".event"
                    ));
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
                Directory.CreateDirectory(String.Concat(
                    StorageDirectory,
                    deckDirectoryName,
                    RemoveIllegalCharactersFromFilename(deck.Title)
                    ));
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    CreateInitialDirectories();
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

        public static Event LoadEventFromDisk(string path)
        {
            string fullpath = String.Concat(StorageDirectory, path);
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
            string fullpath = String.Concat(StorageDirectory, deckDirectoryName, deck.Title);
            deck.Events.Clear();
            foreach (string file in await Task.Run(() => EnumerateFilesTask(fullpath, "*.event")))
            {
                string subDirectory = file.Replace(StorageDirectory, "");
                Event newEvent = await Task.Run(() => LoadEventFromDisk(subDirectory));
                newEvent.IsChanged = false;
                if (newEvent != null)
                    deck.Events.Add(newEvent);
            }
        }

        private static IEnumerable<string> EnumerateFilesTask(string dir, string filetype)
        {
            try
            {
                return Directory.EnumerateFiles(dir, filetype);
            }
            catch
            {
                return new List<string>();
            }
        }

        public static Deck LoadDeckMappingFromDisk(string deckMapping)
        {
            string fullpath = String.Concat(StorageDirectory, deckMapping);
            deckMapping = deckMapping.Replace(deckDirectoryName, "");
            if (!Directory.Exists(fullpath)) return null;
            if (deckMapping.Length <= 0) return null;
            if (deckMapping[0] == '.') return null;
            return new Deck(deckMapping);
        }
        public static async void LoadAllDeckMappingsFromDisk(ObservableCollection<Deck> decks)
        {
            decks.Clear();
            try
            {
                foreach (string folder in await Task.Run(() => EnumerateDirectoriesTask(String.Concat(StorageDirectory, deckDirectoryName))))
                {
                    string subDirectory = folder.Replace(StorageDirectory, "");
                    Deck deck = DataStorage.LoadDeckMappingFromDisk(subDirectory);
                    if (deck != null)
                        decks.Add(deck);
                }
            }
            catch (DirectoryNotFoundException)
            {
                CreateInitialDirectories();
            }
            catch
            {
            }
        }

        private static IEnumerable<string> EnumerateDirectoriesTask(string dir)
        {
            try
            {
                return Directory.EnumerateDirectories(dir);
            }
            catch
            {
                return new List<string>();
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
                Directory.CreateDirectory(String.Concat(
                    StorageDirectory, 
                    deckDirectoryName, 
                    RemoveIllegalCharactersFromFilename(deck.Title), 
                    @"\", 
                    ".Backups"
                    ));
                Directory.Move(
                    String.Concat(
                        StorageDirectory, 
                        deckDirectoryName,
                        RemoveIllegalCharactersFromFilename(deck.Title), 
                        @"\", 
                        RemoveIllegalCharactersFromFilename(ev.Title), 
                        " #", 
                        ev.Id, 
                        ".event"),
                    String.Concat(
                        StorageDirectory,
                        deckDirectoryName,
                        RemoveIllegalCharactersFromFilename(deck.Title),
                        @"\", 
                        @".Backups\", 
                        RemoveIllegalCharactersFromFilename(ev.Title),
                        " #", 
                        ev.Id, 
                        " ", 
                        RemoveIllegalCharactersFromFilename(DateTime.Now.ToString()),
                        ".event"
                    ));
            }
            catch
            {
                System.Windows.MessageBox.Show("Backing up event failed.");
            }
        }
        private static void BackupDeck(Deck deck)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(StorageDirectory, deckDirectoryName, ".Backups"));
                Directory.Move(String.Concat(StorageDirectory, deckDirectoryName, RemoveIllegalCharactersFromFilename(deck.Title), @"\"), String.Concat(StorageDirectory, deckDirectoryName, @".Backups\", RemoveIllegalCharactersFromFilename(deck.Title), " ", RemoveIllegalCharactersFromFilename(DateTime.Now.ToString())));
            }
            catch
            {
                System.Windows.MessageBox.Show("Backing up deck failed.");
            }
        }

        public static void RenameDeck(Deck deck, string newDeckTitle)
        {
            try
            {
                Directory.Move(
                    String.Concat(StorageDirectory, deckDirectoryName, RemoveIllegalCharactersFromFilename(deck.Title), @"\"),
                    String.Concat(StorageDirectory, deckDirectoryName, newDeckTitle, @"\")
                );
            }
            catch
            {
                System.Windows.MessageBox.Show("Moving of deck failed.");
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
