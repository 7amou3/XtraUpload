using Google.Protobuf.WellKnownTypes;
using System;
using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    public static class UserFactory
    {
        public static gUser Convert(this User user)
        {
            if (user == null) return null;

            return new gUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc)),
                LastModified = Timestamp.FromDateTime(DateTime.SpecifyKind(user.LastModified, DateTimeKind.Utc)),
            };
        }
    }
}
