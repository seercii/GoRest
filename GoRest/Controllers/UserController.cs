using GoRest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace GoRest.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<IActionResult> Index()
        {
            List<User> users = await GetUsersFromApi();

            return View(users);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUser user)
        {
            if (ModelState.IsValid)
            {
                await CreateUserInApi(user);
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            User user = await GetUserByIdFromApi(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User user)
        {
          
            if (id != user.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await UpdateUserInApi(user);
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            User user = await GetUserByIdFromApi(id);

            if (user == null)
            {
                return NotFound();
            }
            await DeleteUserInApi(id);
            return RedirectToAction("Index");
        }

       


        private async Task<List<User>> GetUsersFromApi()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer 0dc61835f2071460be8907dfb48b7344adbad42d8f17d3f9c8531d3b8599e878");
                string apiUrl = "https://gorest.co.in/public/v2/users";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(responseData);

                    return users;
                }
                else
                {
                    return new List<User>();
                }
            }
        }

        [HttpPost]
        private async Task CreateUserInApi(CreateUser user)
        {
            string apiUrl = "https://gorest.co.in/public/v2/users";
            var jsonData = JsonConvert.SerializeObject(user);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer 0dc61835f2071460be8907dfb48b7344adbad42d8f17d3f9c8531d3b8599e878");

                await client.PostAsync(apiUrl, stringContent);
            }
        }


        private async Task<User> GetUserByIdFromApi(int id)
        {
            string apiUrl = $"https://gorest.co.in/public/v2/users/{id}";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                User user = JsonConvert.DeserializeObject<User>(responseData);

                return user;
            }

            return null;
        }
        private async Task UpdateUserInApi(User user)
        {
            string apiUrl = $"https://gorest.co.in/public/v2/users/{user.id}";
            var jsonData = JsonConvert.SerializeObject(user);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer 0dc61835f2071460be8907dfb48b7344adbad42d8f17d3f9c8531d3b8599e878");

                await client.PatchAsync(apiUrl, stringContent);
            }
        }
        private async Task DeleteUserInApi(int id)
        {
            string apiUrl = $"https://gorest.co.in/public/v2/users/{id}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer 0dc61835f2071460be8907dfb48b7344adbad42d8f17d3f9c8531d3b8599e878");

                await client.DeleteAsync(apiUrl);
            }
        }

    }
}
