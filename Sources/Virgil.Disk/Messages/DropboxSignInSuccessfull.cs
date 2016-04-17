namespace Virgil.Disk.Messages
{
    using Dropbox.Api;
    using Model;
    using ViewModels;

    public class DropboxSignInSuccessfull
    {
        public DropboxSignInSuccessfull(OAuth2Response oauth)
        {
            this.Result = new DropboxCredentials(oauth);
        }

        public DropboxCredentials Result { get; } 
    }
}