using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyDataModels;
using System.Diagnostics;

namespace MyDataManagerWinForms
{
    public partial class MainForm : Form
    {
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<DataDbContext> _optionsBuilder;

        private IList<Category> Categories = new List<Category>();
        private IList<Item> Items = new List<Item>();

        private IList<FoodGroup> FoodGroups = new List<FoodGroup>();
        private IList<Food> Foods = new List<Food>();
        private IList<Recipe> Recipes = new List<Recipe>();
        private IList<RecipeItem> ReceipeItems = new List<RecipeItem>();

        public MainForm()
        {
            InitializeComponent();
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

            //load categories
            using (var db = new DataDbContext(_optionsBuilder.Options))
            {
                FoodGroups = db.FoodGroups.OrderBy(x => x.Group).ToList();
                //Items = db.Items.ToList();
                Foods = db.Foods.OrderBy(x => x.Name).ToList();
                Recipes = db.Recipes.ToList();
                ReceipeItems = db.ReceipeItems.ToList();

                cboCategories.DataSource = FoodGroups;
                checkedListBox1.DataSource = Foods;

            }
        }

        private void cboCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cboBox = sender as ComboBox;
            var selItem = cboBox.SelectedItem as FoodGroup;

            //LoadGrid(selItem);
        }

        //private void LoadGrid(FoodGroup selectedItem)
        //{
        //    //Debug.WriteLine($"Selected Item {selectedItem.Id}| {selectedItem.Name}");
        //    var curData =  Foods.Where(x => x.FoodGroupId == selectedItem.Id).ToList();
        //    dgItems.DataSource = curData;
        //}



        private void dgItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoadGrid2(Food selectedItem)
        {
            using (var db = new DataDbContext(_optionsBuilder.Options))
            {
                
                var theFood = db.Foods.Include(x => x.RecipeItems).ThenInclude(y => y.Recipe)
                                 .SingleOrDefault(x => x.Id == selectedItem.Id);
                var recipes = new List<Recipe>();

                foreach(var ri in theFood.RecipeItems)
                {
                    recipes.Add(ri.Recipe);
                }
                dgItems.DataSource = recipes;
            }
            
            
            //var firstData = ReceipeItems.Where(x => x.FoodId == selectedItem.Id && x.RecipeId == Recipes.Id).ToList();
            
            
            
            
            
            //var secondData = Recipes.Where(x => x.Id == firstData.RecipeId).ToList();
            
            //dgItems.DataSource = secondData;
        }
      
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckedListBox;
            var selItem = checkBox.SelectedItem as Food;

            LoadGrid2(selItem);
        }
    }
}