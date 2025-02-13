using Farmer_Project.Models.Entity;
using Farmer_Project.Models.ViewModel;
using Farmer_Project.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Farmer_Project.Controllers
{
    public class FarmersController : Controller
    {
        
        private readonly AppDbContext _dbContext; 
        private readonly ImageService _imageService; 


        //建構子
        public FarmersController(AppDbContext dbContext, ImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }


        //部落格文章
        [HttpGet]
        public IActionResult Articles(int FarmersId, int ArticlesId)
        {
            ViewData["BodyClass"] = "sub_page";

            var article = _dbContext.FarmersArticles
                .Include(a => a.FarmersInfo)
                .Include(a => a.FarmersArticlesDetails)
                .Where(a => a.ArticlesId == ArticlesId &&
                            a.FarmersId == FarmersId &&
                            a.IsPublished == true)
                .FirstOrDefault();

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        //小農頁面
        [HttpGet]
        public IActionResult FarmersIndex(string CropsType)
        {
            ViewData["BodyClass"] = "sub_page";

            if (string.IsNullOrWhiteSpace(CropsType))
            {
                ViewBag.Message = "請輸入有效的作物類型！";
                return View(new List<FarmersArticles>());
            }

            try
            {
                var farmers = _dbContext.FarmersArticles
                    .Include(f => f.FarmersInfo)
                    .Include(fd => fd.FarmersArticlesDetails)
                    .Where(a => a.FarmersInfo != null &&
                                a.FarmersInfo.CropsType != null &&
                                a.FarmersInfo.CropsType.ToLower() == CropsType.ToLower() &&
                                a.ArticleType == "小農介紹" &&
                                a.IsPublished == true)
                    .ToList();

                if (!farmers.Any())
                {
                    ViewBag.Message = "沒有找到符合條件的文章！";
                }

                return View(farmers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("查詢發生錯誤：" + ex.Message);
                ViewBag.Message = "發生錯誤，請稍後再試。";
                return View(new List<FarmersArticles>());
            }
        }

        //小農故事
        [HttpGet]
        public IActionResult FarmerDetail(int FarmersId, int ArticlesId)
        {
            ViewData["BodyClass"] = "sub_page";

            var article = _dbContext.FarmersArticles
                        .Include(f => f.FarmersInfo)
                        .Include(fad => fad.FarmersArticlesDetails)
                        .Where(a => a.FarmersId == FarmersId &&
                                    a.ArticlesId == ArticlesId &&
                                    a.IsPublished == true)
                        .FirstOrDefault();

            if (article == null)
            {
                return NotFound("文章尚未建置或不存在");
            }

            return View(article);
        }
        //-- start of 小農基本資料 --//
        //小農資料搜尋
        [HttpPost]
        public IActionResult InfoSearch(string Search)
        {
            // 檢索 FarmersInfo
            var FarmersInfoQuery = _dbContext.FarmersInfo
                                             .Include(fa => fa.FarmersArticles)
                                             .Include(fad => fad.FarmersArticles)
                                             .AsQueryable();

            if (!string.IsNullOrEmpty(Search?.Trim()))
            {
                // 轉換成英文
                var transSearch = TransDictionary(Search);

                // 根據搜尋條件篩選
                FarmersInfoQuery = FarmersInfoQuery.Where(f => f.Name.Contains(Search) ||
                                                               f.FarmName.Contains(Search) ||
                                                               f.CropsType.Contains(transSearch) ||
                                                               f.Address.Contains(Search) ||
                                                               f.Phone.Contains(Search));
            }

            
            var farmers = FarmersInfoQuery.ToList();
            var members = _dbContext.User.ToList();

            var viewModel = new DashboardViewModel
            {
                MemberInfo = members,
                FarmersInfo = farmers
            };

            TempData["show"] = "FarmersInfoTable";

            return View("~/Views/Backend/DashBoard.cshtml", viewModel);
        }

        //取消搜尋
        [HttpPost]
        public IActionResult InfoSearchCancel(string Search)
        {
            var allMembers = _dbContext.User.ToList();
            var allFarmers = _dbContext.FarmersInfo
                                .Include(fa => fa.FarmersArticles)
                                .ThenInclude(fad => fad.FarmersArticlesDetails)
                                .ToList();

            var viewModel = new DashboardViewModel
            {
                MemberInfo = allMembers,
                FarmersInfo = allFarmers
            };

            TempData["show"] = "FarmersInfoTable";
            return View("~/Views/Backend/DashBoard.cshtml", viewModel);
        }



            //小農資料新增
            [HttpGet]
        public IActionResult FarmersInfoAdd()
        {
            var model = new FarmersInfoViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersInfoAdd(FarmersInfoViewModel model)
        {
            try
            {
                if (model.Image != null)
                {
                    model.ImagePath = await _imageService.SaveImageAndToImgPath(model.Image);
                }
                else
                {
                    // 沒有上傳圖片，使用預設圖片路徑
                    model.ImagePath = "/images/f-1.jpg";
                }

                var farmersInfo = new FarmersInfo
                {
                    Name = model.Name,
                    FarmName = model.FarmName,                    
                    Address = model.Address,
                    Phone = model.Phone,
                    Email = model.Email,
                    Image = model.ImagePath,
                    CropsType = model.CropsType,
                    PlantType = model.PlantType
                };

                // 儲存資料到資料庫
                _dbContext.Add(farmersInfo);
                await _dbContext.SaveChangesAsync();

                ViewBag.Message = "資料儲存成功！";
                TempData["show"] = "FarmersInfoTable";
                return RedirectToAction("DashBoard","Backend");
            }
            catch
            {
                ViewBag.Message = "農場名或 Email已有資料，請更換！";
                return View(model);
            }
        }

        //小農資料修改
        [HttpGet]
        public IActionResult FarmersInfoEdit(int id)
        {
            if (id == 0)
            {
                return BadRequest("無效的農場ID");
            }

            var farmer = _dbContext.FarmersInfo.FirstOrDefault(f => f.FarmersId == id);
            
            if (farmer == null)
            {
                return NotFound("找不到對應的農場資料");
            }

            var model = new FarmersInfoViewModel
            {
                FarmersId = farmer.FarmersId,
                Name = farmer.Name,
                FarmName = farmer.FarmName,
                ImagePath = farmer.Image,
                Address = farmer.Address,
                Phone = farmer.Phone,
                Email = farmer.Email,
                CropsType = farmer.CropsType,
                PlantType = farmer.PlantType
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersInfoEdit(int id, FarmersInfoViewModel model)
        {
            if (id != 0)
            {
                var farmer = await _dbContext.FarmersInfo.FirstOrDefaultAsync(f => f.FarmersId == id);

                if (model.Image != null)
                {
                    farmer.Image = await _imageService.SaveImageAndToImgPath(model.Image);
                }
                else 
                {
                    farmer.Image = farmer.Image;
                }

                //更換新資料
                farmer.Name = model.Name;
                farmer.FarmName = model.FarmName;
                farmer.Address = model.Address;
                farmer.Phone = model.Phone;
                farmer.Email = model.Email;
                farmer.CropsType = model.CropsType;
                farmer.PlantType = model.PlantType;
            }

            // 保存變更
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "小農資料修改成功！";

            TempData["show"] = "FarmersInfoTable";            
            return RedirectToAction("DashBoard", "Backend");
        }

        //小農資料刪除
        [HttpPost]
        public async Task<IActionResult> FarmersInfoDelete(int DeleteFarmersId)
        {
            var farmer = await _dbContext.FarmersInfo.FirstOrDefaultAsync(f => f.FarmersId == DeleteFarmersId);
            if (farmer != null)
            {
                // 刪除資料
                _dbContext.FarmersInfo.Remove(farmer);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                return NotFound("找不到對應的農場資料");
            }

            // 設置刪除成功消息
            TempData["SuccessMessage"] = "小農資料刪除成功！";
            TempData["show"] = "FarmersInfoTable";
            return RedirectToAction("DashBoard","Backend");
        }
        //-- end of 小農基本資料 --//


        //-- start of 小農文章 --//
        //小農文章，段落新增
        [HttpGet]
        public IActionResult FarmersBlogAdd(int id)
        {
            var viewModel = new FarmersArticlesViewModel
            {
                FarmersId = id,
                Articles = new ArticleViewModel(),
                ArticleDetails = new List<FarmersArticlesDetailsViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersBlogAdd(FarmersArticlesViewModel model, string action)
        {
            //判斷: "true":發佈 / "false":保存
            bool isPublish = action == "true";

            IFormFile image = model.Articles.ArticleImage; //前端傳來的圖片
            string imagePath = null;
            string subImagePath = null;

            //處理文章圖片
            if (image != null)
            {
                imagePath = await _imageService.SaveImageAndToImgPath(image);
            }
            else
            {
                imagePath = "/images/default.jpg";
            }

            //賦值給Entity
            var ArticleEntity = new FarmersArticles
            {
                FarmersId = model.FarmersId,
                ArticleType = model.Articles.ArticleType ?? "未選擇",
                ArticleTitle = model.Articles.ArticleTitle ?? "未上傳",
                ArticleImagePath = imagePath,
                ArticleSummary = model.Articles.ArticleSummary ?? "未上傳",
                IsPublished = isPublish
            };


            //資料庫保存
            _dbContext.FarmersArticles.Add(ArticleEntity);
            await _dbContext.SaveChangesAsync();

            //文章ID
            var articleID = ArticleEntity.ArticlesId;

            //處理段落
            foreach (var detail in model.ArticleDetails)
            {
                //處理段落圖片
                if (detail.SubImage != null)
                {               
                    subImagePath = await _imageService.SaveImageAndToImgPath(detail.SubImage);
                }
                else
                {
                    subImagePath = "/images/default.jpg";
                }

                //段落儲存
                var articleDetail = new FarmersArticlesDetails
                {
                    ArticlesId = articleID,
                    DetailId = detail.DetailId,
                    SubTitle = detail.SubTitle ?? "未上傳",
                    SubImagePath = subImagePath,
                    SubContent = detail.SubContent ?? "未上傳"
                };
                await _dbContext.FarmersArticlesDetails.AddAsync(articleDetail);
            }
            await _dbContext.SaveChangesAsync();

            TempData["show"] = "FarmersArticlesList";
            return RedirectToAction("DashBoard", "Backend");
        }

        //小農文章，段落編輯
        [HttpGet]
        public async Task<IActionResult> FarmersBlogEdit(int id) 
        {
            //根據ArticleId撈文章
            var article = await _dbContext.FarmersArticles
                            .Include(a => a.FarmersArticlesDetails) //撈段落資料
                            .FirstOrDefaultAsync(a => a.ArticlesId == id);

            var model = new FarmersArticlesViewModel
            {
                FarmersId = (int)article.FarmersId,
                ArticlesId = article.ArticlesId,
                Articles = new ArticleViewModel
                {
                    ArticleType = article.ArticleType,
                    ArticleTitle = article.ArticleTitle,
                    ArticleImagePath = article.ArticleImagePath,
                    ArticleSummary = article.ArticleSummary
                },

                ArticleDetails = article.FarmersArticlesDetails.Select(detail => new FarmersArticlesDetailsViewModel
                {
                    DetailId = detail.DetailId,
                    SubTitle = detail.SubTitle,
                    SubImagePath = detail.SubImagePath,
                    SubContent = detail.SubContent
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FarmersBlogEdit(int id, FarmersArticlesViewModel model, string action) 
        {
            //判斷: "true":發佈 / "false":保存
            bool isPublish = action == "true";


            //檢查文章
            var article = await _dbContext.FarmersArticles
                .Include(a => a.FarmersArticlesDetails) //撈段落資料
                .FirstOrDefaultAsync(a => a.ArticlesId == model.ArticlesId && a.FarmersId == model.FarmersId); //判斷該文章是否該小農文章

            if (article == null)
            {
                return NotFound("文章或農民不存在，或者文章不屬於該農民。");
            }


            IFormFile image = model.Articles.ArticleImage;
            string imagePath = null;

            //處理文章圖片
            if (image != null)
            {
                imagePath = await _imageService.SaveImageAndToImgPath(image);
            }
            else
            {
                imagePath = article.ArticleImagePath; //使用原本圖片
            }

            //更新文章內容
            article.ArticleType = model.Articles.ArticleType ?? "未選擇";
            article.ArticleTitle = model.Articles.ArticleTitle ?? "未填寫";
            article.ArticleSummary = model.Articles.ArticleSummary ?? "未填寫";
            article.ArticleImagePath = imagePath ?? "未上傳";
            article.IsPublished = isPublish;

            // 處理段落
            foreach (var detailData in model.ArticleDetails)
            {
                // 找對應的段落
                var detail = article.FarmersArticlesDetails.FirstOrDefault(d => d.DetailId == detailData.DetailId);
                string subImagePath = null;

                // 如果段落存在，則更新它
                if (detail != null)
                {
                    if (detailData.SubImage != null)
                    {
                        subImagePath = await _imageService.SaveImageAndToImgPath(detailData.SubImage);
                    }
                    else
                    {
                        subImagePath = detail.SubImagePath;  // 如果沒有新的圖片，使用原本的圖片路徑
                    }

                    // 更新段落
                    detail.SubTitle = detailData.SubTitle ?? "未填寫";
                    detail.SubImagePath = subImagePath;
                    detail.SubContent = detailData.SubContent ?? "未填寫";
                }
                else if (detail == null)
                {
                    // 若段落不存在，則新增段落
                    var newDetail = new FarmersArticlesDetails
                    {
                        ArticlesId = (int)model.ArticlesId,
                        DetailId = detailData.DetailId,
                        SubTitle = detailData.SubTitle ?? "未填寫",
                        SubImagePath = subImagePath ?? "未上傳",
                        SubContent = detailData.SubContent ?? "未填寫"
                    };

                    if (detailData.SubImage != null)
                    {
                        newDetail.SubImagePath = await _imageService.SaveImageAndToImgPath(detailData.SubImage);
                    }
                    else
                    {
                        newDetail.SubImagePath = "/images/default.jpg";
                    }

                    // 新增段落到文章中
                    article.FarmersArticlesDetails.Add(newDetail);
                }
            }

            //提交的段落
            var submittedDetailIds = model.ArticleDetails.Select(d => d.DetailId).ToList();

            //撈出的段落 排除提交的段落(多的段落-刪除)
            var detailsToRemove = article.FarmersArticlesDetails.Where(d => !submittedDetailIds.Contains(d.DetailId)).ToList();

            foreach (var item in detailsToRemove)
            {
                article.FarmersArticlesDetails.Remove(item);
            }

            // 儲存更改
            await _dbContext.SaveChangesAsync();

            TempData["show"] = "FarmersArticlesList";

            return RedirectToAction("DashBoard", "Backend", new { farmersId = model.FarmersId});
        }
 
 
        //文章刪除
        [HttpPost]
        public async Task<IActionResult> FarmersBlogDelete(int DeleteArticlesId)
        {
            var article = await _dbContext.FarmersArticles.FirstOrDefaultAsync(a => a.ArticlesId == DeleteArticlesId);

            if (article == null)
            {
                return NotFound("該文章不存在！");
            }

            _dbContext.FarmersArticles.Remove(article);
            await _dbContext.SaveChangesAsync();


            TempData["show"] = "FarmersArticlesList";
            return RedirectToAction("Dashboard", "Backend");
        }


        //------start of 一般方法-------------------------------------------------------------------------------------------------//       

        // 定義中文到英文的映射
        private string TransDictionary(string input)
        {
            // 如果是英文，直接返回
            if (input.All(c => char.IsLetter(c) && char.IsAscii(c)))
            {
                return input;
            }

            var translateDict = new Dictionary<string, string>
            {
                { "咖啡", "Coffee" },
                { "芒果", "Mango" }
            };

            // 如果字典中有對應的中文，則返回對應的英文，否則返回原始的中文
            if (translateDict.TryGetValue(input, out var translated))
            {
                return translated;
            }

            return input;
        }


        //------end of 一般方法-------------------------------------------------------------------------------------------------//
    }


}
