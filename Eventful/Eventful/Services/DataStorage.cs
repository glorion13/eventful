using Eventful.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eventful.Services
{
    public static class DataStorage
    {
        public static string DefaultStorageDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Eventful\\";
        public static string StorageDirectory = DefaultStorageDirectory;

        private static string deckDirectoryName = "decks\\";
        private static string tagDirectoryName = "tags\\";
        private static string variableDirectoryName = "variables\\";

        private static async Task<bool> CreateInitialDirectories()
        {
            try
            {
                Directory.CreateDirectory(StorageDirectory);
                Directory.CreateDirectory($"{StorageDirectory}{deckDirectoryName}");
                Directory.CreateDirectory($"{StorageDirectory}{tagDirectoryName}");
                Directory.CreateDirectory($"{StorageDirectory}{variableDirectoryName}");
                return true;
            }
            catch
            {
                await ViewModel.MessageWindowsViewModel.ShowOkMessage("Problem reading/writing to disk", "There is a chance your data will not be saved.");
                return false;
            }
        }

        private static string GetEventFileName(Event ev) => $"{ev.Title} #{ev.Id}.event";
        private static string GetDeckFilePath(Deck deck) => $"{StorageDirectory}{deckDirectoryName}{deck.Title}\\";

        public static bool SaveVariableToDisk(Variable variable)
        {
            if (StorageDirectory == null) return false;
            if (variable == null) return false;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Variable));
                TextWriter textWriter = new StreamWriter($"{variableDirectoryName}{variable.Title}");
                serializer.Serialize(textWriter, variable);
                textWriter.Close();
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    CreateInitialDirectories();
                    SaveVariableToDisk(variable);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SaveEventToDisk(Event ev, Deck deck)
        {
            if (deck == null || ev == null || StorageDirectory == null) return false;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Event));
                TextWriter textWriter = new StreamWriter($"{GetDeckFilePath(deck)}{GetEventFileName(ev)}");
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
            catch (Exception)
            {
                return false;
            }
        }
        public static bool SaveDeckToDisk(Deck deck)
        {
            try
            {
                Directory.CreateDirectory(GetDeckFilePath(deck));
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
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Event));
                TextReader textReader = new StreamReader($"{StorageDirectory}{path}");
                Event ev = new Event();
                ev = (Event)deserializer.Deserialize(textReader);
                foreach (Screen screen in ev.Screens)
                {
                    screen.ParentEvent = ev;
                    screen.IsSelected = false;
                    foreach (Option option in screen.Options)
                    {
                        option.Source = screen;
                        option.UpdateTargetFromId();
                    }
                }
                textReader.Close();
                return ev;
            }
            catch
            {
                return null;
            }
        }
        
        public static async Task<ObservableCollection<Event>> LoadDeckEventsFromDisk(Deck deck)
        {
            ObservableCollection<Event> events = new ObservableCollection<Event>();
            string fullpath = GetDeckFilePath(deck);
            try
            {
                foreach (string file in EnumerateFilesTask(fullpath, "*.event"))
                {
                    string subDirectory = file.Replace(StorageDirectory, "");
                    Event newEvent = LoadEventFromDisk(subDirectory);
                    if (newEvent != null)
                    {
                        newEvent.IsChanged = false;
                        events.Add(newEvent);
                    }
                    if (newEvent == null)
                        await ViewModel.MessageWindowsViewModel.ShowOkMessage("Problem Loading Event", $"Event from file {subDirectory} was not loaded successfully.");
                }
                return events;
            }
            catch
            {
                await ViewModel.MessageWindowsViewModel.ShowOkMessage("Problem Loading Deck", "An error occurred while trying to load the deck's events.");
                return events;
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

        private static Deck CreateDeckMapping(string deckMapping)
        {
            string fullpath = $"{StorageDirectory}{deckMapping}";
            deckMapping = deckMapping.Replace(deckDirectoryName, "");
            if (!Directory.Exists(fullpath)) return null;
            if (deckMapping.Length <= 0) return null;
            if (deckMapping[0] == '.') return null;
            return new Deck(deckMapping);
        }
        public static ObservableCollection<Deck> LoadAllDeckMappingsFromDisk()
        {
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
            try
            {
                foreach (string folder in EnumerateDirectoriesTask(string.Concat(StorageDirectory, deckDirectoryName)))
                {
                    string subDirectory = folder.Replace(StorageDirectory, "");
                    Deck deck = CreateDeckMapping(subDirectory);
                    if (deck != null)
                        decks.Add(deck);
                }
                return decks;
            }
            catch (DirectoryNotFoundException)
            {
                CreateInitialDirectories();
                return decks;
            }
            catch
            {
                return decks;
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

        public static bool DeleteEvent(Event ev, Deck deck)
        {
            return BackupEvent(ev, deck);
        }
        public static bool DeleteDeck(Deck deck)
        {
            return BackupDeck(deck);
        }

        private static string GetDateTime()
        {
            return DateTime.Now.ToString().Replace("/", "-").Replace(":", "-");
        }

        private static bool BackupEvent(Event ev, Deck deck)
        {
            try
            {
                Directory.CreateDirectory(string.Concat(GetDeckFilePath(deck), ".Backups"));
                Directory.Move(
                    $"{GetDeckFilePath(deck)}{GetEventFileName(ev)}",
                    $"{GetDeckFilePath(deck)}.Backups\\{GetDateTime()} {GetEventFileName(ev)}"
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static bool BackupDeck(Deck deck)
        {
            try
            {
                string deckBackupsDirectory = $"{StorageDirectory}{deckDirectoryName}.Backups\\";
                Directory.CreateDirectory(deckBackupsDirectory);
                Directory.Move(
                    GetDeckFilePath(deck),
                    $"{deckBackupsDirectory}{deck.Title} {GetDateTime()}"
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RenameDeck(Deck deck, string newDeckTitle)
        {
            try
            {
                Directory.Move(
                    GetDeckFilePath(deck),
                    $"{StorageDirectory}{deckDirectoryName}{newDeckTitle}\\"
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RenameEvent(Event ev, Deck deck, string newEventTitle)
        {
            try
            {
                Event newEvent = new Event(ev);
                newEvent.Title = newEventTitle;
                newEvent.Id = ev.Id;
                newEvent.IsChanged = false;
                bool success = SaveEventToDisk(newEvent, deck);
                if (success)
                    DeleteEvent(ev, deck);
                return success;
            }
            catch
            {
                return false;
            }
        }
    }
}
