using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyDataModels;
using System.Diagnostics;

namespace MyDataManagerWinForms
{
    public delegate void RespondToMessageEvent(string message);

    public partial class MainForm : Form
    {
        private static IConfigurationRoot _configuration;
        public static DbContextOptionsBuilder<DataDbContext> _optionsBuilder;

        private IList<Category> Categories = new List<Category>();
        private IList<Item> Items = new List<Item>();

        private IList<FoodGroup> FoodGroups = new List<FoodGroup>();
        public IList<Food> Foods = new List<Food>();
        private IList<Recipe> Recipes = new List<Recipe>();
        private IList<RecipeItem> ReceipeItems = new List<RecipeItem>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void RespondToMessage(string m)
        {
            MessageBox.Show(m);
            Refresh();
        }

        public void Refresh()
        {
            //load categories
            using (var db = new DataDbContext(_optionsBuilder.Options))
            {
                FoodGroups = db.FoodGroups.OrderBy(x => x.Group).ToList();
                Foods = db.Foods.OrderBy(x => x.Name).ToList();
                Recipes = db.Recipes.ToList();
                ReceipeItems = db.ReceipeItems.ToList();

                cboCategories.DataSource = FoodGroups;
                checkedListBox1.DataSource = Foods;
            }
        }

        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("MyDataManagerData"));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            BuildOptions();
            Refresh();            
        }

        private void cboCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cboBox = sender as ComboBox;
            var selItem = cboBox.SelectedItem as FoodGroup;

            LoadGrid(selItem);
        }

        private void LoadGrid(FoodGroup selectedItem)
        {
            //Debug.WriteLine($"Selected Item {selectedItem.Id}| {selectedItem.Name}");
            var curData =  Foods.Where(x => x.FoodGroupId == selectedItem.Id).ToList();
            dgItems.DataSource = curData;
        }



        private void dgItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoadGrid2(Food selectedItem)
        {
            using (var db = new DataDbContext(_optionsBuilder.Options))
            {
                
                var theFood = db.Foods.Include(x => x.RecipeItems).ThenInclude(y => y.Recipe)
                                 .Select(x => new
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Recipes = x.RecipeItems.Select(y => y.Recipe)
                                 } )
                                 .SingleOrDefault(x => x.Id == selectedItem.Id);
                var recipes = new List<Recipe>();

                /*foreach(var ri in theFood.RecipeItems)
                {
                    recipes.Add(ri.Recipe);
                }*/
                dgItems.DataBindings.Clear();
                dgItems.DataSource = theFood.Recipes;
            }
            
            
            //var firstData = ReceipeItems.Where(x => x.FoodId == selectedItem.Id && x.RecipeId == Recipes.Id).ToList();
            
            
            
            
            
            //var secondData = Recipes.Where(x => x.Id == firstData.RecipeId).ToList();
            
            //dgItems.DataSource = secondData;
        }
      
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckedListBox;
            var selItem = checkBox.SelectedItem as Food;

            //LoadGrid2(selItem);
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            //var moreFood = new AddFood();
            var moreFood = new AddorUpdate();
            moreFood._respondToMessageEvent += new RespondToMessageEvent(RespondToMessage);
            moreFood.ShowDialog();
        }
    }
}