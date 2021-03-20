using AutoMapper;
using CrownBlog.DAL;
using CrownBlog.Models.ViewModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Synology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace CrownBlog
{
    public class Startup
    {
        /// <summary>
        /// �]�w��
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// �غc��
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<BlogContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Blogs"]));
            services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs }));

            services.AddMvc();
            services.AddLogging();
            services.AddSynology();

            #region Automapper

            services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ArticleRequestBody, BlogArticle>();
                cfg.CreateMap<ArticleResponseBody, BlogArticle>();
                cfg.CreateMap<BlogTag, TagItem>();
                cfg.CreateMap<BlogMessage, MessageItem>();
                cfg.CreateMap<ArticleResponseBody, ArticleRequestBody>();
                cfg.CreateMap<TagRequestBody, BlogTag>();

            })));


            #endregion

            #region Cors

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:44395/").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("https://www.crownchung.tw/").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BlogContext blogContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Blog}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });

            blogContext.Database.EnsureCreated();
        }
    }
}