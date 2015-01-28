using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
namespace WindowsFormsApplication9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
private void LoadAlbums()
{
    // Get all albums, including photos, from the database
    ReadOnlyCollection<album> albums = Photo.GetPhotoAlbums();

    // Now iterate through them and add to treeview
    foreach(Album album in albums)
    {
        TreeNode albumNode = new TreeNode(album.Name);
                
        // Add the album struct to the Tag for later
        // retrieval of info without database call
        albumNode.Tag = album;

        treeAlbums.Nodes.Add(albumNode);

        // Add each photo in album to treenode for the album
        foreach(Photo photo in album.Photos)
        {
            TreeNode photoNode = new TreeNode(photo.Name);
            photoNode.Tag = photo;

            albumNode.Nodes.Add(photoNode);
        }                
        public static ReadOnlyCollection<album> _GetPhotoAlbums()
{
    List<album> albums = new List<album>();
   
                while(reader.Read())
                {
                    albums.Add(new Album()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2)
                    }
                    );
                }
private void OnAddPhoto(object sender, EventArgs e)
{
    if(DialogResult.OK == openFileDialog1.ShowDialog())
    {
        // Retrieve the Album to add photo(s) to
        Album album = (Album)treeAlbums.SelectedNode.Tag;

        // We allow multiple selections so loop through each one
        foreach(string file in openFileDialog1.FileNames)
        {
            // Create a new stream to load this photo into
            System.IO.FileStream stream = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            // Create a buffer to hold the stream bytes
            byte[] buffer = new byte[stream.Length];
            // Read the bytes from this stream
            stream.Read(buffer, 0, (int)stream.Length);
            // Now we can close the stream
            stream.Close();

            Photo photo = new Photo()
            {
                // Extract out the name of the file an use it for the name
                // of the photo
                Name = System.IO.Path.GetFileNameWithoutExtension(file),
                Image = buffer
            };

            // Insert the image into the database and add it to the tree
            Data.AddPhoto(album.Id, photo);
            buffer = null;

            // Add the photo to the album node
            TreeNode node = treeAlbums.SelectedNode.Nodes.Add(photo.Name);
            node.Tag = photo;
        }
    
   
        private void AfterSelect(object sender, TreeViewEventArgs e)
{
    DisplayPanel.Visible = true;

    if(treeAlbums.SelectedNode.Tag is Album)
    {
        Album album = (Album)treeAlbums.SelectedNode.Tag;

        DisplayName.Text = album.Name;
        DisplayDescription.Text = album.Description;

        pictureBox.Image = null;
    }
    else if(treeAlbums.SelectedNode.Tag is Photo)
    {
        Photo photo = (Photo)treeAlbums.SelectedNode.Tag;

        DisplayName.Text = photo.Name;
        DisplayDescription.Text = photo.Description;

        System.IO.MemoryStream stream = new System.IO.MemoryStream(photo.Image, true);
        stream.Write(photo.Image, 0, photo.Image.Length);

        // Draw photo to scale of picturebox
        DrawToScale(new Bitmap(stream));
    }
    else
    {
        DisplayPanel.Visible = false;
    }

    }
}
