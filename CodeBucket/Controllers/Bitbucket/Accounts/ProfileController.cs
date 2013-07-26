using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using MonoTouch.Dialog.Utilities;
using CodeBucket.Bitbucket.Controllers.Followers;
using CodeBucket.Bitbucket.Controllers.Events;
using CodeBucket.Bitbucket.Controllers.Groups;
using CodeBucket.Bitbucket.Controllers.Repositories;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeFramework.Elements;

namespace CodeBucket.Bitbucket.Controllers
{
	public class ProfileController : Controller<UsersModel>, IImageUpdated
	{
        private HeaderView _header;

        public string Username { get; private set; }

		public ProfileController(string username, bool push = true) 
            : base(push)
		{
            Title = username;
			Username = username;
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _header = new HeaderView(View.Bounds.Width) { Title = Username };
            Root.Add(new Section(_header));

            var followers = new StyledElement("Followers", () => NavigationController.PushViewController(new UserFollowersController(Username), true), Images.Heart);
            var events = new StyledElement("Events", () => NavigationController.PushViewController(new EventsController(Username), true), Images.Buttons.Event);
            var groups = new StyledElement("Groups", () => NavigationController.PushViewController(new GroupController(Username), true), Images.Buttons.Group);
            var repos = new StyledElement("Repositories", () => NavigationController.PushViewController(new RepositoryController(Username, true) { Model = Model.Repositories }, true), Images.Repo);
            Root.Add(new [] { new Section { followers, events, groups }, new Section { repos } });
        }

        protected override void OnRefresh()
        {
            _header.Subtitle = Model.User.FirstName ?? "" + " " + (Model.User.LastName ?? "");
            _header.Image = ImageLoader.DefaultRequestImage(new System.Uri(Model.User.Avatar), this);
            BeginInvokeOnMainThread(() => _header.SetNeedsDisplay());

            if (Username.Equals(Application.Account.Username, System.StringComparison.OrdinalIgnoreCase))
            {
                Application.Account.AvatarUrl = Model.User.Avatar;
                BeginInvokeOnMainThread(() => Application.Accounts.Update(Application.Account));
            }
        }

        protected override UsersModel OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].GetInfo(forced);
        }

        public void UpdatedImage (System.Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }
	}
}

