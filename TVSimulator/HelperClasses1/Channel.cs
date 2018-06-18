using MediaClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HelperClasses
{
    public class Channel
    {
        
        public enum channelType
        {
            local,
            youtube_playlist,
            youtube_channel,
        }

        private int channelNumber;
        private string typeOfMedia;       //movies,tv serias,music or youtube stream
        private string genre;             //the name should be channel main genre
        private List<Media> media;
        private Database db;
        private DateTime StartCycleTime;
        private string youtubeChannelID;
        private string youtubeChannelName;
        private int youtubeVideoIndex = 0;
        private List<YoutubeVideo> youtubeVideoList;
        private Dictionary<DateTime, Media> boardSchedule;
        private int id;
        private channelType mChannelType;

        public const int PROMO_TIME = 7;

        public List<Media> Media { get => media; set => media = value; }
        public int ChannelNumber { get => channelNumber; set => channelNumber = value; }
        public string Genre { get => genre; set => genre = value; }
        public string TypeOfMedia { get => typeOfMedia; set => typeOfMedia = value; }
        public string YoutubeChannelID { get => youtubeChannelID; set => youtubeChannelID = value; }
        public int YoutubeVideoIndex { get => youtubeVideoIndex; set => youtubeVideoIndex = value; }
        public List<YoutubeVideo> YoutubeVideoList { get => youtubeVideoList; set => youtubeVideoList = value; }
        public int Id { get => id; set => id = value; }
        public Dictionary<DateTime, Media> BoardSchedule { get => boardSchedule; set => boardSchedule = value; }
        public channelType MChannelType { get => mChannelType; set => mChannelType = value; }
        public string YoutubeChannelName { get => youtubeChannelName; set => youtubeChannelName = value; }



        //private /*MediaSchedule*/ schedule;

        public Channel()
        { }

        public Channel(int channelNumber, string typeOfMedia, string genre)
        {
            this.channelNumber = channelNumber;
            this.genre = genre;
            this.typeOfMedia = typeOfMedia;

            this.media = new List<Media>();

            this.db = new Database();
            this.StartCycleTime = DateTime.Parse(Constants.START_CYCLE);

            this.YoutubeVideoList = new List<YoutubeVideo>();
            SetType();
            //this.schedule = schedule;
            //TODO:playNow and playNext is done when the schedule will be ready
        }


      


        public void buildSchedule()
        {
            if (typeOfMedia.Equals(Constants.MOVIE))
                buildMovieSchedule();
            if (typeOfMedia.Equals(Constants.TVSERIES))
                buildTVSchedule();
            if (typeOfMedia.Equals(Constants.YOUTUBE_CHANNEL))
                buildYoutubeSchedule();
            if (typeOfMedia.Equals(Constants.YOUTUBE_PLAYLIST_CHANNEL))
                buildYoutubePlaylistSchedule();
        }

        public void buildMovieSchedule()
        {
            List<Movie> movies = db.getMovieList();

            foreach (Movie m in movies)
                if (m.getFirstGenre().Equals(genre))
                    media.Add((Media)m);
        }

        public void buildTVSchedule()
        {
            List<TvSeries> tvs = db.getTVList();

            foreach (TvSeries t in tvs)
                if (t.getFirstGenre().Equals(genre))
                    media.Add((Media)t);
        }


        public int buildYoutubeSchedule()
        {
            var ytbVideos = db.getYoutubeVideosfromChannel(YoutubeChannelID);
            if (ytbVideos == null || ytbVideos.Count() == 0)
                return -1;
        
            foreach (YoutubeVideo ytbvid in ytbVideos)
                media.Add(ytbvid);
         
            return 0;
        }


        public int buildYoutubePlaylistSchedule()
        {
            double sum = 0;
            var durTime = new TimeSpan();

            var tempmedia = db.getYoutubePlaylistVideosFromPlaylist(youtubeChannelID);
            if (tempmedia == null && tempmedia.Count()==0)
                return -1;
            foreach (Media ytbPlsVid in tempmedia)
            {
                media.Add(ytbPlsVid);
                durTime = ytbPlsVid.getDurationTimespan();
                sum += durTime.TotalSeconds;
            }
            return 0;
        }


        public void addMedia()
        {
            if (typeOfMedia.Equals(Constants.MOVIE))
            {
                List<Media> mediaVideos = db.getAllMediaList();

                if (genre == "General")
                {
                    foreach (Media med in mediaVideos)
                        media.Add(med);
                    return;
                }
                List<Movie> movies = db.getMovieList();

                foreach (Movie m in movies)
                    if (m.getFirstGenre().Equals(genre))
                        media.Add((Media)m);
            }
            if (typeOfMedia.Equals(Constants.TVSERIES))
            {
                List<TvSeries> tvs = db.getTVList();

                foreach (TvSeries t in tvs)
                    if (t.getFirstGenre().Equals(genre))
                        media.Add((Media)t);

                /*var c = createFakeMedia();
                media.AddRange(c);*/
            }
            if(typeOfMedia.Equals(Constants.YOUTUBE_CHANNEL))
            {
                var ytbVideos = db.getYoutubeVideosfromChannel(YoutubeChannelID);
                if (ytbVideos == null)
                    return;
                foreach(YoutubeVideo vid in ytbVideos)
                    media.Add(vid);
                
            }
            if(typeOfMedia.Equals(Constants.YOUTUBE_PLAYLIST_CHANNEL))
            {
                media = db.getYoutubePlaylistVideosFromPlaylist(youtubeChannelID);
            }
        }

        public int bs(DateTime startDate)          // michael hoo michael 
        {
            Dictionary<DateTime, Media> board = new Dictionary<DateTime, Media>();
            int numOfDays = 7;
            var startTime = db.getTimes().StartTime;        // init lists from DB
            var endTime = db.getTimes().EndTime;            // init lists from DB
            DateTime date = startDate;
            DateTime temp;

            TimeSpan mediaDuration;
            var i = 0;
            var j = 0;  // run all over the movies
            var k = 0;
            var mediaLength = media.Count;
            if (mediaLength == 0)
                return -1;
            
            //create live broadcast media

            while (k < numOfDays || j < mediaLength)
            {
                if (board.Count > Constants.MAX_DICT_KEYS)
                    break;
                i = (int)date.DayOfWeek;

                DateTime finalHourDate = new DateTime(date.Year, date.Month, date.Day);
                finalHourDate = finalHourDate.AddHours(endTime[i]);     //end time in one day

                mediaDuration = media[j % mediaLength].getDurationTimespan();
                if (mediaDuration.TotalSeconds > 0)
                {
                    temp = date.Add(mediaDuration);                     // current date + media duration

                    if (DateTime.Compare(temp, finalHourDate) < 0)     //if temp is earlier than finalhourDate
                    {
                        board.Add(date, media[j % mediaLength]);
                        date = temp;
                        date = date.AddSeconds(PROMO_TIME);
                        j++;
                    }
                    else
                    {
                        temp = date.AddDays(1);
                        i = (int)temp.DayOfWeek;
                        date = new DateTime(temp.Year, temp.Month, temp.Day, startTime[i], 0, 0);
                        k++;
                    }
                }
                else
                {
                    j++;
                }
            }

            board = createRepeatMedia(startDate, board,startTime,endTime);
            BoardSchedule = board;
            return 0;
        }


        public void bTVs(DateTime startDate)
        {
            Dictionary<DateTime, Media> board = new Dictionary<DateTime, Media>();
            int numOfDays = 7;
            var startTime = db.getTimes().StartTime;        // init lists from DB
            var endTime = db.getTimes().EndTime;            // init lists from DB
            DateTime date = startDate;
            DateTime temp;
            TimeSpan mediaDuration;
            int longestSeries, mediaLength;
            List<int> capOfLists;

            List<List<TvSeries>> listOfSeries;                          // list of all tv series sorted by seasons and episodes 
            listOfSeries = sortTvSeries();

            capOfLists = createCapList(listOfSeries);  // list of capacities of tv seiries lists
            longestSeries = getMostLength(listOfSeries);            // number of episodes of the longest tv series
            mediaLength = listOfSeries.Count();
           
            var i = 0;  // run over days
            var j = 0;  // run all over the movies
            var k = 0;  //count 7 days at least

            int g = 0;

            while (k < numOfDays || g < longestSeries)
            {
                i = (int)date.DayOfWeek;

                DateTime finalHourDate = new DateTime(date.Year, date.Month, date.Day);
                finalHourDate = finalHourDate.AddHours(endTime[i]);     //end time in one day

                int curTvS = (g / 2) % mediaLength;
                if (capOfLists[curTvS] < 0)
                    capOfLists[curTvS] = listOfSeries[curTvS].Count() - 1;

                mediaDuration = listOfSeries[curTvS].ElementAt(capOfLists[curTvS]).getDurationTimespan();
                temp = date.Add(mediaDuration);                     // current date + media duration

                if (DateTime.Compare(temp, finalHourDate) < 0)     //if temp is earlier than finalhourDate
                {
                    board.Add(date, listOfSeries[curTvS].ElementAt(capOfLists[curTvS]));
                    date = temp;
                    date = date.AddSeconds(PROMO_TIME);
                    capOfLists[curTvS] -= 1;
                    g++;
                }
                else
                {
                    temp = date.AddDays(1);
                    i = (int)temp.DayOfWeek;
                    date = new DateTime(temp.Year, temp.Month, temp.Day, startTime[i], 0, 0);
                    k++;
                }
            }
            board = createRepeatMedia(startDate, board, startTime, endTime);

            BoardSchedule = board;
        }


        public int bTVs_YT(DateTime startDate)
        {
            Dictionary<DateTime, Media> board = new Dictionary<DateTime, Media>();
            int numOfDays = 7;
            var startTime = db.getTimes().StartTime;        // init lists from DB
            var endTime = db.getTimes().EndTime;            // init lists from DB
            DateTime date = startDate;
            DateTime temp;
            TimeSpan mediaDuration;
            int longestSeries, mediaLength;
            List<int> capOfLists;

            List<List<YoutubePlaylistVideo>> listOfSeries_YT = sortTvSeries_YoutubePlaylist();                          // list of all tv series sorted by seasons and episodes 
            if (listOfSeries_YT == null)
                return -1;

            capOfLists = createCapList_YT(listOfSeries_YT);  // list of capacities of tv seiries lists
            longestSeries = getMostLength_YT(listOfSeries_YT);            // number of episodes of the longest tv series
            mediaLength = listOfSeries_YT.Count();
            if (mediaLength == 0)
                return -1;

            var i = 0;  // run over days
            var j = 0;  // run all over the movies
            var k = 0;  //count 7 days at least

            int g = 0;

            while (k < numOfDays || g < longestSeries)
            {
                i = (int)date.DayOfWeek;

                DateTime finalHourDate = new DateTime(date.Year, date.Month, date.Day);
                finalHourDate = finalHourDate.AddHours(endTime[i]);     //end time in one day

                int curTvS = (g / 2) % mediaLength;
                
                if (TypeOfMedia.Equals(Constants.YOUTUBE_PLAYLIST_CHANNEL))
                {
                    if (capOfLists[curTvS] < 0)
                        capOfLists[curTvS] = listOfSeries_YT[curTvS].Count() - 1;

                    mediaDuration = listOfSeries_YT[curTvS].ElementAt(capOfLists[curTvS]).getDurationTimespan();
                    if (mediaDuration.TotalSeconds == 0)
                        return -1;
                    temp = date.Add(mediaDuration);                     // current date + media duration

                    if (DateTime.Compare(temp, finalHourDate) < 0)     //if temp is earlier than finalhourDate
                    {
                        board.Add(date, listOfSeries_YT[curTvS].ElementAt(capOfLists[curTvS]));
                        date = temp;
                        date = date.AddSeconds(PROMO_TIME);
                        capOfLists[curTvS] -= 1;
                        g++;
                    }
                    else
                    {
                        temp = date.AddDays(1);
                        i = (int)temp.DayOfWeek;
                        date = new DateTime(temp.Year, temp.Month, temp.Day, startTime[i], 0, 0);
                        k++;
                    }
                }
            }
            board = createRepeatMedia(startDate, board, startTime, endTime);

            BoardSchedule = board;
            return 0;
        }



        private Dictionary<DateTime, Media> createRepeatMedia(DateTime startDate, Dictionary<DateTime, Media> board, int[] startTime, int[] endTime)
        {
            //create repeat broadcast media
            var i = 0;
            var j = 0;
            DateTime date = startDate;
            DateTime temp;
            // set the last date for adding media until this date 
            var fd = board.Keys.Last().AddDays(1);          //day after final day 
            var stfs = startTime[(int)fd.DayOfWeek];        //start time of final day
            DateTime finalDate = new DateTime(fd.Year, fd.Month, fd.Day, stfs, 0, 0);

            Dictionary<DateTime, Media> tempList = new Dictionary<DateTime, Media>();

            while (DateTime.Compare(date, finalDate) < 0)
            {
                for (i = 0; i < board.Count; i++)
                {
                    var day = board.Keys.ElementAt(i).Day;
                    var month = board.Keys.ElementAt(i).Month;
                    var year = board.Keys.ElementAt(i).Year;
                    var hour = board.Keys.ElementAt(i).Hour;
                    if (date.Year == year && date.Month == month && date.Day == day && date.Hour <= hour )
                        tempList.Add(board.Keys.ElementAt(i), board.Values.ElementAt(i));
                }
                if (tempList.Count() == 0 || tempList.Values.Last() == null )
                {
                    tempList.Add(board.Keys.ElementAt(0), board.Values.ElementAt(0));
                }
                temp = tempList.Keys.Last().Add(tempList.Values.Last().getDurationTimespan());      // start time to add repeat media

                DateTime startHourDate = new DateTime(date.Year, date.Month, date.Day);
                startHourDate = startHourDate.AddDays(1);
                startHourDate = startHourDate.AddHours(startTime[(int)startHourDate.DayOfWeek]);     //start time in one day

                while (DateTime.Compare(temp, startHourDate) < 0)     //if temp is earlier than startHourDate
                {
                    board.Add(temp, tempList.ElementAt(j % tempList.Count).Value);
                    var mediaDur = tempList.ElementAt(j % tempList.Count).Value.getDurationTimespan();
                    temp = temp.Add(mediaDur);
                    temp = temp.AddSeconds(PROMO_TIME);
                    j++;
                }
                tempList.Clear();
                date = startHourDate;
                j = 0;
            }

            var l = board.OrderBy(key => key.Key);
            board = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            return board;
        }


        private List<int> createCapList(List<List<TvSeries>> listOfSeries)
        {
            List<int> cap = new List<int>();
            for (var i = 0; i < listOfSeries.Count(); i++)
                cap.Add(listOfSeries[i].Count()-1);
            return cap;
        }

        private List<int> createCapList_YT(List<List<YoutubePlaylistVideo>> listOfSeries)
        {
            List<int> cap = new List<int>();
            for (var i = 0; i < listOfSeries.Count(); i++)
                cap.Add(listOfSeries[i].Count() - 1);
            return cap;
        }

        private int getMostLength(List<List<TvSeries>> listOfSeries) // dont try to understand that alone..
        {
            int noe = 0;    //num of episodes
            int index = -1;
            for (var i = 0; i < listOfSeries.Count(); i++)
                if (noe <= listOfSeries[i].Count())
                {
                    noe = listOfSeries[i].Count();
                    index = i;
                }

            int len = listOfSeries.Count();
            int del,x;

            //maximum of episodes in the board is (neo/2)*length of the tv series lists *2 

            if (noe % 2 == 0)
            {
                del = noe / 2;
                x = 2 * len * del - (len * 2 - (2 * index+1)) + 1;
            }
            else
            {
                del = (noe + 1) / 2;
                x = 2 * len * del - (len * 2 - (2 * index+1));
            }

            return x;
        }


        private int getMostLength_YT(List<List<YoutubePlaylistVideo>> listOfSeries) // dont try to understand that alone..
        {
            int noe = 0;    //num of episodes
            int index = -1;
            for (var i = 0; i < listOfSeries.Count(); i++)
                if (noe <= listOfSeries[i].Count())
                {
                    noe = listOfSeries[i].Count();
                    index = i;
                }

            int len = listOfSeries.Count();
            int del, x;

            //maximum of episodes in the board is (neo/2)*length of the tv series lists *2 

            if (noe % 2 == 0)
            {
                del = noe / 2;
                x = 2 * len * del - (len * 2 - (2 * index + 1)) + 1;
            }
            else
            {
                del = (noe + 1) / 2;
                x = 2 * len * del - (len * 2 - (2 * index + 1));
            }

            return x;
        }

        private List<List<TvSeries>> sortTvSeries()
        {
            List<Media> SortedList = media.OrderBy(o => o.Name).ToList();
            var name = SortedList[0].Name;
            List<List<TvSeries>> all = new List<List<TvSeries>>();
            var first = new List<TvSeries>();
            first.Add(SortedList[0] as TvSeries);
            all.Add(first);

            for(var i=1;i<SortedList.Count();i++)       // add each series to one list
            {
                if (SortedList[i].Name.Equals(name))
                    all.Last().Add(SortedList[i] as TvSeries);
                else
                {
                    name = SortedList[i].Name;
                    var newTvSeries = new List<TvSeries>();
                    newTvSeries.Add(SortedList[i] as TvSeries);
                    all.Add(newTvSeries);
                }
            }

            for(var j=0;j<all.Count;j++)    //sorted by seasons and episodes - order by desc
            {
                all[j] = all[j].OrderByDescending(o => int.Parse(o.Episode)).ToList();
                all[j] = all[j].OrderByDescending(o => int.Parse(o.Season)).ToList();
            }
            return all;
        }

        private List<List<YoutubePlaylistVideo>> sortTvSeries_YoutubePlaylist()
        {
            if (media.Count() == 0)
                return null;
            List<Media> SortedList = media.OrderBy(o => o.Name).ToList();
            var name = SortedList[0].Name;
            List<List<YoutubePlaylistVideo>> all = new List<List<YoutubePlaylistVideo>>();
            var first = new List<YoutubePlaylistVideo>();
            first.Add(SortedList[0] as YoutubePlaylistVideo);
            all.Add(first);

            for (var i = 1; i < SortedList.Count(); i++)       // add each series to one list
            {
                if (SortedList[i].Name.Equals(name))
                    all.Last().Add(SortedList[i] as YoutubePlaylistVideo);
                else
                {
                    name = SortedList[i].Name;
                    var newTvSeries = new List<YoutubePlaylistVideo>();
                    newTvSeries.Add(SortedList[i] as YoutubePlaylistVideo);
                    all.Add(newTvSeries);
                }
            }

            for (var j = 0; j < all.Count; j++)    //sorted by seasons and episodes - order by desc
            {
                all[j] = all[j].OrderByDescending(o => int.Parse(o.Episode)).ToList();
                all[j] = all[j].OrderByDescending(o => int.Parse(o.Season)).ToList();
            }
            return all;
        }




        public List<Media> createFakeMedia()
        {
            Random rnd = new Random();
            var numOfTvSeries = 5;
            List<Media> tv = new List<Media>();
            string tname = "a";
            TimeSpan dur = new TimeSpan(0, 30, 00);
            for (var i=0;i<numOfTvSeries;i++)
            {
                int numOfEpisodes = rnd.Next(10, 24);
                for (var j = 0; j < numOfEpisodes; j++)
                {
                    if (i % 5 == 0)
                        tname = "b";
                    if (i % 5 == 1)
                        tname = "t";
                    if (i % 5 == 2)
                        tname = "c";
                    if (i % 5 == 3)
                        tname = "far";
                    if (i % 5 == 4)
                        tname = "a";
                    tv.Add(new TvSeries("somePath", tname, dur.ToString(), "Action", "01", "" + (j+1)));
                }
            }

            return tv;
        }


        // helper function
        private void SetType()
        {
            switch (TypeOfMedia)
            {
                case Constants.YOUTUBE_CHANNEL:
                    mChannelType = channelType.youtube_channel;
                    break;
                case Constants.YOUTUBE_PLAYLIST_CHANNEL:
                    mChannelType = channelType.youtube_playlist;
                    break;
                default:
                    mChannelType = channelType.local;
                    break;
            }
        }
    }
}

