using AutoMapper;
using CrownBlog.DAL;
using CrownBlog.Filters;
using CrownBlog.Models.ViewModel;
using CrownBlog.TokenAuthentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
        private readonly string _AllowAllGetPolicy = "AllowAllGetPolicy";
        private readonly string _AllowAllPostPolicy = "AllowAllPostPolicy";
        private readonly string _AllowAllPutPolicy = "AllowAllPutPolicy";
        private readonly string _AllowAllDeletePolicy = "AllowAllDeletePolicy";

        /// <summary>
        /// 設定檔
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ITokenManager, TokenManager>();

            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<BlogContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Blogs"]));
            services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs }));

            //services.AddScoped<JwtAuthFilter>();
            //services.AddControllers(Configuration =>
            //{
            //    Configuration.Filters.Add<JwtAuthFilter>();
            //});
            services.AddMvc();
            services.AddLogging();

            //services.AddSynology();

            #region Automapper

            services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ArticleRequestBody, BlogArticle>();
                cfg.CreateMap<ArticleResponseBody, BlogArticle>();
                cfg.CreateMap<BlogTag, TagItem>();
                cfg.CreateMap<BlogMessage, MessageItem>();
                cfg.CreateMap<MessageRequestBody, BlogMessage>();
                cfg.CreateMap<ArticleResponseBody, ArticleRequestBody>();
                cfg.CreateMap<TagRequestBody, BlogTag>();
                cfg.CreateMap<Member,  MemberModel>();
                cfg.CreateMap<MemberModel, MemberResponseBody>();
                cfg.CreateMap<MemberRequestBody, Member>();

            })));


            #endregion

            #region Cors

            services.AddCors(options =>
            {
                options.AddPolicy(_AllowAllGetPolicy,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .WithMethods("GET");
                    });
                options.AddPolicy(_AllowAllPutPolicy,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .WithMethods("PUT");
                    });
                options.AddPolicy(_AllowAllPostPolicy,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .WithMethods("Post");
                    });
                options.AddPolicy(_AllowAllDeletePolicy,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .WithMethods("Delete");
                    });
            });

            #endregion

            #region Auth
            //services.AddAuthentication().AddJwtBearer(options =>
            //{
            //    options.Audience = "http"
            //});
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
                app.UseExceptionHandler("/api/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(_AllowAllGetPolicy);

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
