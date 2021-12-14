using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace Evote_Service.Model.Repository
{
    public class UserRepository : IUserRepository
    {
        private EvoteContext _evoteContext;
        private readonly IHttpClientFactory _clientFactory;
        public UserRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, ISMSRepository sMSRepository, IEmailRepository emailRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _clientFactory = clientFactory;

        }
        public async Task<UserEntity> getEvent(string lineId)
        {

            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).Include(i => i.eventVoteEntities).First();

            return userEntity;
        }
    }
}
