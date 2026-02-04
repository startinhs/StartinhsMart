using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using StartinhsMart.CoreService.Categories;
using StartinhsMart.CoreService.Permissions;
using StartinhsMart.CoreService.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Volo.Abp.Content;
using Volo.Abp.Http.Client;

namespace StartinhsMart.CoreService.Blazor.Client.Pages
{
    public partial class Categories
    {
        private IReadOnlyList<CategoryDto> CategoryList { get; set; } = new List<CategoryDto>();
        private IReadOnlyList<CategoryDto> ParentCategoryLookupList { get; set; } = new List<CategoryDto>();
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = "SortOrder, Name";
        private int TotalCount { get; set; }
        private bool CanCreateCategory { get; set; }
        private bool CanEditCategory { get; set; }
        private bool CanDeleteCategory { get; set; }
        private CategoryCreateDto NewCategory { get; set; }
        private Validations CreateCategoryValidations { get; set; } = new();
        private CategoryUpdateDto EditingCategory { get; set; }
        private Validations EditCategoryValidations { get; set; } = new();
        private Guid EditingCategoryId { get; set; }
        private Modal CreateCategoryModal { get; set; } = new();
        private Modal EditCategoryModal { get; set; } = new();
        private GetCategoriesInput Filter { get; set; }
        private DataGrid<CategoryDto> CategoriesDataGrid { get; set; } = new();
        private bool ShowAdvancedFilters { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        
        private const long MaxCategoryImageFileUploadSize = 10 * 1024 * 1024;
        private bool OnNewCategoryImageLoading = false;
        private bool OnEditCategoryImageLoading = false;

        public Categories()
        {
            NewCategory = new CategoryCreateDto();
            EditingCategory = new CategoryUpdateDto();
            Filter = new GetCategoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await GetCategoriesAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Categories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["NewCategory"], async () =>
            {
                await OpenCreateCategoryModal();
            }, IconName.Add, requiredPolicyName: CoreServicePermissions.Categories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCategory = await AuthorizationService.IsGrantedAsync(CoreServicePermissions.Categories.Create);
            CanEditCategory = await AuthorizationService.IsGrantedAsync(CoreServicePermissions.Categories.Edit);
            CanDeleteCategory = await AuthorizationService.IsGrantedAsync(CoreServicePermissions.Categories.Delete);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CategoryDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task GetCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CategoriesAppService.GetListAsync(Filter);
            CategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private async Task OpenCreateCategoryModal()
        {
            NewCategory = new CategoryCreateDto();
            await LoadParentCategoriesAsync();
            await CreateCategoryValidations.ClearAll();
            await CreateCategoryModal.Show();
        }

        private async Task CloseCreateCategoryModal()
        {
            NewCategory = new CategoryCreateDto();
            await CreateCategoryModal.Hide();
        }

        private async Task CreateCategoryAsync()
        {
            try
            {
                if (await CreateCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await CategoriesAppService.CreateAsync(NewCategory);
                await GetCategoriesAsync();
                await CloseCreateCategoryModal();
                await UiMessageService.Success(L["SavedSuccessfully"]);
            }
            catch (Exception ex)
            {
                await UiMessageService.Error(GetErrorMessage(ex), title: L["AnErrorOccurred"]);
            }
        }

        private async Task OpenEditCategoryModal(CategoryDto category)
        {
            var categoryDto = await CategoriesAppService.GetAsync(category.Id);
            EditingCategoryId = categoryDto.Id;
            EditingCategory = ObjectMapper.Map<CategoryDto, CategoryUpdateDto>(categoryDto);
            await LoadParentCategoriesAsync(category.Id);
            await EditCategoryValidations.ClearAll();
            await EditCategoryModal.Show();
        }

        private async Task CloseEditCategoryModal()
        {
            EditingCategory = new CategoryUpdateDto();
            EditingCategoryId = Guid.Empty;
            await EditCategoryModal.Hide();
        }

        private async Task UpdateCategoryAsync()
        {
            try
            {
                if (await EditCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await CategoriesAppService.UpdateAsync(EditingCategoryId, EditingCategory);
                await GetCategoriesAsync();
                await CloseEditCategoryModal();
                await UiMessageService.Success(L["UpdatedSuccessfully"]);
            }
            catch (Exception ex)
            {
                await UiMessageService.Error(GetErrorMessage(ex), title: L["AnErrorOccurred"]);
            }
        }

        private async Task DeleteCategoryAsync(CategoryDto category)
        {
            var confirmMessage = L["ItemWillBeDeletedMessage"];
            if (await UiMessageService.Confirm(confirmMessage))
            {
                try
                {
                    await CategoriesAppService.DeleteAsync(category.Id);
                    await GetCategoriesAsync();
                    await UiMessageService.Success(L["DeletedSuccessfully"]);
                }
                catch (Exception ex)
                {
                    await UiMessageService.Error(GetErrorMessage(ex), title: L["AnErrorOccurred"]);
                }
            }
        }

        private string GetErrorMessage(Exception ex)
        {
            return ex?.Message ?? L["AnErrorOccurred"];
        }

        private async Task LoadParentCategoriesAsync(Guid? excludeId = null)
        {
            var result = await CategoriesAppService.GetListAsync(new GetCategoriesInput
            {
                MaxResultCount = 1000,
                Sorting = "SortOrder, Name"
            });
            
            ParentCategoryLookupList = result.Items
                .Where(c => !excludeId.HasValue || c.Id != excludeId.Value)
                .ToList();
        }

        private async Task OnNewCategoryImageChanged(InputFileChangeEventArgs e)
        {
            try
            {  
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxCategoryImageFileUploadSize)
                {
                    await UiMessageService.Error(L["UploadFailedMessage"]);
                    return;
                }

                OnNewCategoryImageLoading = true;
                await InvokeAsync(StateHasChanged);
                
                var result = await UploadFileAsync(e.File!);

                NewCategory.ImageId = result.Id;
                OnNewCategoryImageLoading = false;
                await InvokeAsync(StateHasChanged);
            }
            catch(Exception ex)
            {
                OnNewCategoryImageLoading = false;
                await UiMessageService.Error(GetErrorMessage(ex), title: L["AnErrorOccurred"]);
            }
        }

        private async Task OnEditCategoryImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxCategoryImageFileUploadSize)
                {
                    await UiMessageService.Error(L["UploadFailedMessage"]);
                    return;
                }

                OnEditCategoryImageLoading = true;
                await InvokeAsync(StateHasChanged);
                
                var result = await UploadFileAsync(e.File!);

                EditingCategory.ImageId = result.Id;
                OnEditCategoryImageLoading = false;
                await InvokeAsync(StateHasChanged);
            }
            catch(Exception ex)
            {
                OnEditCategoryImageLoading = false;
                await UiMessageService.Error(GetErrorMessage(ex), title: L["AnErrorOccurred"]);
            }
        }

        private async Task<AppFileDescriptorDto> UploadFileAsync(IBrowserFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(long.MaxValue).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                
                return await CategoriesAppService.UploadFileAsync(new RemoteStreamContent(ms, file.Name, file.ContentType));
            }
        }

        private async Task DownloadFileAsync(Guid fileId)
        {
            var token = (await CategoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("CoreService") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/categories/file?DownloadToken={token}&FileId={fileId}", forceLoad: true);
        }

        private string GetImageUrl(Guid fileId)
        {
            var remoteService = RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("CoreService").Result ?? RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default").Result;
            return $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/categories/image?ImageId={fileId}";
        }
    }
}

