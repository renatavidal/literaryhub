// BLL/BLLBooks.cs
using System.Collections.Generic;
using BE;
using MPP;

namespace BLL
{
    public static class BLLBooks
    {
        private static readonly MPPBook _mBook = new MPPBook();

        public static List<BEBook> GetWantToRead(int userId)
        {
            return _mBook.GetByUserStatus(userId, (byte)BEBook.UserBookStatus.WantToRead);
        }

        public static List<BEBook> GetRead(int userId)
        {
            return _mBook.GetByUserStatus(userId, (byte)BEBook.UserBookStatus.Read);
        }

        public static void MoveToRead(int userId, int bookId)
        {
            _mBook.MoveToRead(userId, bookId);
        }

        public static void RemoveFromLists(int userId, int bookId, string list)
        {
            byte status = (list == "read") ? (byte)BEBook.UserBookStatus.Read : (byte)BEBook.UserBookStatus.WantToRead;
            _mBook.Remove(userId, bookId, status);
        }

        public static int EnsureAndAdd(
            int userId,
            string googleVolumeId,
            string title,
            string authors,
            string thumbnailUrl,
            string isbn13,
            string publishedDate,
            string list)
        {
            var be = new BEBook();
            be.GoogleVolumeId = googleVolumeId;
            be.Title = title;
            be.Authors = authors;
            be.ThumbnailUrl = thumbnailUrl;
            be.Isbn13 = isbn13;
            be.PublishedDate = publishedDate;

            int bookId = _mBook.AddIfNotExists(be); 
            byte status = (list == "read") ? (byte)BEBook.UserBookStatus.Read : (byte)BEBook.UserBookStatus.WantToRead;
            _mBook.Add(userId, bookId, status);
            return bookId;
        }
    }
}
