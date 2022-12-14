using System;
using Eto.Forms;
using Eto.Drawing;
using Npgsql;

namespace cpts_451_yelp
{
    // Class for the Login window.
    public partial class Login : Form
    {
        // Click event handler
        public event EventHandler<EventArgs> Click;

        // Selection event handler for boxes
        public event EventHandler<EventArgs> SelectedValueChanged;

        // Bunch of gross variables.
        DynamicLayout layout = new DynamicLayout(); // Layout for the page
        TextBox nameBox = new TextBox // For user to log-in
        {
            PlaceholderText = "Name"
        };

        ListBox nameList = new ListBox // Box of searched names
        {
            Size = new Size(205, 150)
        };

        Button search = new Button // button to search for name
        {
            Text = "Search"
        };
        SharedInfo s = new SharedInfo(); // Shared info

        public UserInfo currentUser = new UserInfo(); // To store user information

        // Main entry point for user window.
        public Login(UserInfo inUser) // Main Form
        {
            Title = "Login"; // Title of Application
            MinimumSize = new Size(200, 300); // Default resolution

            createUI(); // Puts everything where it belongs
            this.Content = layout; // Instantiates the layout

            currentUser = inUser;

            // Events are attached to event handlers here
            search.Click += new EventHandler<EventArgs>(queryName);
            nameList.SelectedValueChanged += new EventHandler<EventArgs>(setUser);
            nameList.SelectedValueChanged += new EventHandler<EventArgs>(queryUserInfo);
        }

        // Queries the userid based on the name entered
        public void queryName(object sender, EventArgs e)
        {
            nameList.Items.Clear(); // Clears the box

            // Query to select userid
            string cmd = @"SELECT Users.userid, username FROM Users WHERE username = '" + nameSearch() + "'";
            s.executeQuery(cmd, queryNameHelper, true);
        }
        public void queryUserInfo(object sender, EventArgs e)
        {
            string cmd = @"SELECT averageStars, yelpingSince, tipCount, totalLikes, fans, funny, cool, useful, latitude, longitude  
                        FROM Users, UserRating, UserLocation WHERE Users.userID = UserLocation.UserID AND Users.userID = '" + currentUser.UserID + "' AND UserRating.userID = '" + currentUser.UserID + "';";
            s.executeQuery(cmd, userInfoHelper, true);
        }

        // Converts the text to a string
        public String nameSearch()
        {
            return nameBox.Text.ToString();
        }

        // Sets the user in userinfo depending on which
        // userid was selected
        public void setUser(object sender, EventArgs e)
        {
            if (nameList.SelectedIndex > -1) // Checks if one was selected
            {
                currentUser.UserID = nameList.SelectedValue.ToString();
                currentUser.Username = nameBox.Text.ToString();
                MessageBox.Show("Logged in as: " + currentUser.Username);
            }
        }

        private void userInfoHelper(NpgsqlDataReader R)
        {
            currentUser.avgStars = Math.Round(R.GetDouble(0), 2);
            currentUser.date = R.GetDateTime(1);
            currentUser.tipCount = R.GetInt32(2);
            currentUser.likes = R.GetInt32(3);
            currentUser.fans = R.GetInt32(4);
            currentUser.funny = R.GetInt32(5);
            currentUser.cool = R.GetInt32(6);
            currentUser.useful = R.GetInt32(7);
            currentUser.UserLat = R.GetDouble(8);
            currentUser.UserLong = R.GetDouble(9);
        }

        // Sets the list of names
        public void queryNameHelper(NpgsqlDataReader R)
        {
            nameList.Items.Add(R.GetString(0));
        }

        // Required for clicks
        protected virtual void OnClick()
        {
            EventHandler<EventArgs> handler = Click;
            if (null != Handler) handler(this, EventArgs.Empty);
        }

        // Required for box selection
        protected virtual void OnSelectedValueChanged()
        {
            EventHandler<EventArgs> handler = SelectedValueChanged;
            if (null != Handler) handler(this, EventArgs.Empty);
        }

        // Puts all of the stuff where it belongs.
        public void createUI()
        {
            layout.DefaultSpacing = new Size(5, 5);
            layout.Padding = new Padding(10, 10, 10, 10);

            layout.BeginHorizontal();

            layout.BeginVertical();
            layout.BeginGroup("Set Current User", new Padding(10, 10, 10, 10));
            layout.BeginHorizontal();
            layout.BeginVertical(padding: new Padding(0, 0, 0, 10));
            layout.AddAutoSized(new Label { Text = "Search User Name" });
            layout.BeginHorizontal();
            layout.AddAutoSized(nameBox);
            layout.AddAutoSized(search);
            layout.EndHorizontal();
            layout.EndVertical();
            layout.EndHorizontal();
            layout.BeginHorizontal();
            layout.BeginVertical(padding: new Padding(0, 0, 0, 10));
            layout.AddAutoSized(new Label { Text = "User IDs" });
            layout.AddAutoSized(nameList);
            layout.EndVertical();
            layout.EndHorizontal();
            layout.EndGroup();
            layout.EndVertical();

            layout.EndHorizontal();
        }

    }
}

