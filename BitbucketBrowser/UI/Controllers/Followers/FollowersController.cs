using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Linq;
using System.Threading;
using BitbucketSharp;
using System.Collections.Generic;
using MonoTouch.Dialog.Utilities;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;


namespace BitbucketBrowser.UI.Controllers.Followers
{
	public abstract class FollowersController : Controller<List<FollowerModel>>
    {
		protected FollowersController()
			: base(true, true)
		{
            Style = UITableViewStyle.Plain;
            Title = "Followers";
            EnableSearch = true;
            AutoHideSearch = true;
		}

        protected override List<FollowerModel> OnOrder(List<FollowerModel> item)
        {
            return item.OrderBy(a => a.Username).ToList();
        }

        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(s => {
                StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                sec.Add(sse);
            });

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }
	}
}
