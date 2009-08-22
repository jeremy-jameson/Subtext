#region Disclaimer/Info
///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Subtext.Framework;
using Subtext.Framework.Components;
using Subtext.Web.Properties;

namespace Subtext.Web.Admin.Pages
{
	public partial class EditImage : AdminPage
	{
		protected const string VSKEY_IMAGEID = "ImageID";
		protected int _imageID;
		protected Image _image;
		protected string _galleryTitle;

		#region Accessors
		private int ImageId
		{
			get
			{
                if (ViewState[VSKEY_IMAGEID] == null || NullValue.NullInt32 == (int)ViewState[VSKEY_IMAGEID])
                {
                    if (null != Request.QueryString[Keys.QRYSTR_IMAGEID])
                        ViewState[VSKEY_IMAGEID] = Convert.ToInt32(Request.QueryString[Keys.QRYSTR_IMAGEID]);
                }
			    return (int)ViewState[VSKEY_IMAGEID];
			}
		}

		public Image Image
		{
			get 
			{
                if (_image == null)
                {
                    _image = Images.GetSingleImage(ImageId, false);
                }

			    if (_image == null)
                    throw new InvalidOperationException(Resources.InvalidOperation_ImageUndefined);
			    
			    return _image;
			}
		}

		#endregion
	    
	    public EditImage()
	    {
            this.TabSectionId = "Galleries";
	    }

		protected void Page_Load(object sender, EventArgs e)
		{
		}
	    
	    public override void DataBind()
	    {
            BindImage();
            base.DataBind();
	    }
	    

		private void BindImage()
		{
			if (NullValue.NullInt32 != ImageId)
			{
                ICollection<LinkCategory> selectionList = Links.GetCategories(CategoryType.ImageCollection, ActiveFilter.None);
				if (selectionList.Count > 0)
				{
					ddlGalleries.DataSource = selectionList;
					ddlGalleries.DataValueField = "Id";
					ddlGalleries.DataTextField = "Title";
					
					lnkThumbnail.ImageUrl = EvalImageUrl(Image);
					lnkThumbnail.NavigateUrl = EvalImageNavigateUrl(Image);
					lnkThumbnail.Visible = true;

					ckbPublished.Checked = Image.IsActive;

					SetGalleryInfo(Image);

					ddlGalleries.DataBind();

                    var listItem = ddlGalleries.Items.FindByValue(_image.CategoryID.ToString(CultureInfo.InvariantCulture));
                    if (listItem != null) {
                        ddlGalleries.SelectedIndex = ddlGalleries.Items.IndexOf(listItem);
                    }
					// HACK: we're disabling this until we do something with/around the provider
					// that will let us actually move the files too.
					ddlGalleries.Enabled = false;

					Advanced.Collapsed = Preferences.AlwaysExpandAdvanced;

					if(AdminMasterPage != null) {
                        string title = string.Format(CultureInfo.InvariantCulture, Resources.EditGalleries_EditImage, Image.Title);
                        AdminMasterPage.Title = title;
					}
				}
				else
				{
					ImageDetails.Visible = false;
                    this.Messages.ShowError(Resources.EditGalleries_MustHaveOneGallery);
				}
			}
			else
			{	
				ImageDetails.Visible = false;
                this.Messages.ShowError(Resources.EditGalleries_ImageIdentifierMissing);
			}
		}

		protected void SetGalleryInfo(Image image)
		{
			_galleryTitle = this.SubtextContext.Repository.GetLinkCategory(image.CategoryID,false).Title;
		}

		protected string EvalImageUrl(object imageObject)
		{
            if (imageObject is Image)
            {
                Image image = (Image)imageObject;
                return Url.ImageUrl(image);
            }
            return String.Empty;
		}

		protected string EvalImageNavigateUrl(object imageObject)
		{
			if (imageObject is Image)
			{
				Image image = (Image)imageObject;
				return Url.GalleryImageUrl(image);
			}
			return String.Empty;
		}

		protected string GetImageGalleryUrl()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}?{1}={2}", Constants.URL_EDITGALLERIES, 
				Keys.QRYSTR_CATEGORYID, Image.CategoryID);
		}

		private void UpdateImage()
		{
			if (Page.IsValid)
			{
				_image = Images.GetSingleImage(this.ImageId, false);
				_image.CategoryID = Convert.ToInt32(ddlGalleries.SelectedItem.Value);
				_image.Title = txbTitle.Text;
				_image.IsActive = ckbPublished.Checked;
				
				try
				{
					Images.UpdateImage(_image);

					// would need to also move files for this to work here. should happen
					// in the provider though.

                    this.Messages.ShowMessage(Resources.EditGalleries_ImageUpdated);
					BindImage();
				}
				catch(Exception ex)
				{
					this.Messages.ShowError(String.Format(Constants.RES_EXCEPTION, "TODO...", ex.Message));
				}
			}
		}

		private void ReplaceImage()
		{
			if (Page.IsValid)
			{
				_image = Images.GetSingleImage(this.ImageId, false);
				_image.CategoryID = Convert.ToInt32(ddlGalleries.SelectedItem.Value);
				_image.Title = txbTitle.Text;
				_image.IsActive = ckbPublished.Checked;
				
				try
				{
					_image.FileName = Path.GetFileName(ImageFile.PostedFile.FileName);
                    _image.LocalDirectoryPath = Url.GalleryDirectoryPath(Blog, _image.CategoryID);
					Images.Update(_image, Images.GetFileStream(ImageFile.PostedFile));

                    this.Messages.ShowMessage(Resources.EditGalleries_ImageUpdated);
					BindImage();
				}
				catch (Exception ex)
				{
					this.Messages.ShowError(String.Format(Constants.RES_EXCEPTION, "TODO...", ex.Message));
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

			ViewState[VSKEY_IMAGEID] = NullValue.NullInt32;
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		protected void lbkReplaceImage_Click(object sender, EventArgs e)
		{
			ReplaceImage();
		}

		protected void lkbUpdateImage_Click(object sender, EventArgs e)
		{
			UpdateImage();		
		}
	}
}
