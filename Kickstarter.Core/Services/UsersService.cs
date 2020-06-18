using Kickstarter.Data;
using Kickstarter.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace Kickstarter.Core.Services
{
    public static class UsersService
    {
        /// <summary>
        /// Returns the status code of the request
        /// </summary>
        /// <param name="followerId">User following</param>
        /// <param name="userId">User to be followed</param>
        /// <returns>HTTP status code of the operation</returns>
        public static async Task<HttpStatusCode> FollowUser(uint followerId, uint userId)
        {
            HttpStatusCode resultCode;
            using (var dbContext = new SQLDbContext())
            {
                if (await dbContext.User.FindAsync(userId) == null)
                {
                    resultCode = HttpStatusCode.NotFound;
                }
                else
                {
                    await dbContext.FollowerToUser.AddAsync(new FollowerToUser
                    {
                        FollowerId = followerId,
                        UserId = userId,
                    });

                    await dbContext.SaveChangesAsync();

                    resultCode = HttpStatusCode.OK;
                }
            }
            return resultCode;
        }
    }
}
