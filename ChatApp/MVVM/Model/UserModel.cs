using System.Diagnostics.CodeAnalysis;

namespace ChatClient.MVVM.Model
{
    public class UserModel
    {
        public string? Username { get; set; }
        public Guid? UID { get; set; }

    }

    public class UserModelEqualityComparer : IEqualityComparer<UserModel>
    {
        public bool Equals(UserModel? x, UserModel? y)
        {

            if (x == null || y == null) { return false; }
            if (x.UID == null || y.UID == null) { return false; }
            if (ReferenceEquals(x, y)) return true;
            return x.UID == y.UID;
        }

        public int GetHashCode([DisallowNull] UserModel obj)
        {
            return obj.UID.GetHashCode();
        }
    }

   
}
