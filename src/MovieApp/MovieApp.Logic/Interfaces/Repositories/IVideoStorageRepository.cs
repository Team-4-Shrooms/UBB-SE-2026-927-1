using MovieApp.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Defines methods for storing video reels in a storage system.
    /// </summary>
    public interface IVideoStorageRepository
    {
        /// <summary>
        /// Uploads a video file from the local disk, inserts it into the database, and returns the stored ReelModel.
        /// </summary>
        /// <param name="reel">The reel to upload.</param>
        Task<Reel> InsertReelAsync(Reel reel);
    }
}
