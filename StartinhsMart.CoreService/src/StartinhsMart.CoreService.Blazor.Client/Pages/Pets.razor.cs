using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using StartinhsMart.CoreService.Pets;
using StartinhsMart.CoreService.Categories;
using StartinhsMart.CoreService.Permissions;
using StartinhsMart.CoreService.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;
using System.Net.Http;
using System.Text.Json;



namespace StartinhsMart.CoreService.Blazor.Client.Pages
{
    public partial class Pets
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        
        [Inject]
        protected HttpClient Http { get; set; } = default!;
        
        [Inject]
        protected ICategoriesAppService CategoriesAppService { get; set; } = default!;
            
        private IJSObjectReference? _jsObjectRef;
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<PetDto> PetList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePet { get; set; }
        private bool CanEditPet { get; set; }
        private bool CanDeletePet { get; set; }
        private PetCreateDto NewPet { get; set; }
        private Validations NewPetValidations { get; set; } = new();
        private PetUpdateDto EditingPet { get; set; }
        private Validations EditingPetValidations { get; set; } = new();
        private Guid EditingPetId { get; set; }
        private Modal CreatePetModal { get; set; } = new();
        private Modal EditPetModal { get; set; } = new();
        private GetPetsInput Filter { get; set; }
        private DataGridEntityActionsColumn<PetDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "pet-create-tab";
        protected string SelectedEditTab = "pet-edit-tab";
        private PetDto? SelectedPet;
        
        private IReadOnlyList<CategoryDto> CategoryLookupList { get; set; } = new List<CategoryDto>();
        
        
        
        
        
        private List<PetDto> SelectedPets { get; set; } = new();
        private bool AllPetsSelected { get; set; }
        
        // Image Search Variables
        private string ImagePreview = "";
        private string PredictionResult = "";
        private byte[] ImageData;
        private bool ShowImageSearch = false;
        private bool isPredicting = false;
        private bool OnImageSearchLoading = false;
        private Modal WebcamModal { get; set; } = new();
        
        // Image modal properties
        private bool showImageModal = false;
        private string enlargedImageUrl = "";
        private double currentZoom = 1.0;
        private const double ZOOM_STEP = 0.2;
        private const double MIN_ZOOM = 0.5;
        private const double MAX_ZOOM = 3.0;
        
        public Pets()
        {
            NewPet = new PetCreateDto();
            EditingPet = new PetUpdateDto();
            Filter = new GetPetsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PetList = new List<PetDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await LoadCategoriesAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _jsObjectRef = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Pages/Pets.razor.js");
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Pets"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPet"], async () =>
            {
                await OpenCreatePetModalAsync();
            }, IconName.Add, requiredPolicyName: CoreServicePermissions.Pets.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePet = await AuthorizationService
                .IsGrantedAsync(CoreServicePermissions.Pets.Create);
            CanEditPet = await AuthorizationService
                            .IsGrantedAsync(CoreServicePermissions.Pets.Edit);
            CanDeletePet = await AuthorizationService
                            .IsGrantedAsync(CoreServicePermissions.Pets.Delete);
                            
                            
        }

        private async Task GetPetsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PetsAppService.GetListAsync(Filter);
            PetList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPetsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await PetsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("CoreService") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/pets/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&CategoryId={Filter.CategoryId}&Name={HttpUtility.UrlEncode(Filter.Name)}&Breed={HttpUtility.UrlEncode(Filter.Breed)}&AgeMin={Filter.AgeMin}&AgeMax={Filter.AgeMax}&Gender={HttpUtility.UrlEncode(Filter.Gender)}&Color={HttpUtility.UrlEncode(Filter.Color)}&WeightMin={Filter.WeightMin}&WeightMax={Filter.WeightMax}&HealthStatus={HttpUtility.UrlEncode(Filter.HealthStatus)}&VaccinationsMin={Filter.VaccinationsMin}&VaccinationsMax={Filter.VaccinationsMax}&Description={HttpUtility.UrlEncode(Filter.Description)}&PriceMin={Filter.PriceMin}&PriceMax={Filter.PriceMax}&QuantityMin={Filter.QuantityMin}&QuantityMax={Filter.QuantityMax}&IsBooth={Filter.IsBooth}&IsStock={Filter.IsStock}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PetDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPetsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePetModalAsync()
        {
            NewPet = new PetCreateDto{
                
                
            };

            SelectedCreateTab = "pet-create-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("clearInputFiles");
            await NewPetValidations.ClearAll();
            await CreatePetModal.Show();
        }

        private async Task CloseCreatePetModalAsync()
        {
            NewPet = new PetCreateDto{
                
                
            };
            await CreatePetModal.Hide();
        }

        private async Task OpenEditPetModalAsync(PetDto input)
        {
            SelectedEditTab = "pet-edit-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("clearInputFiles");
            var pet = await PetsAppService.GetAsync(input.Id);
            
            EditingPetId = pet.Id;
            EditingPet = ObjectMapper.Map<PetDto, PetUpdateDto>(pet);
            HasSelectedPetImage = EditingPet.ImageId != null && EditingPet.ImageId != Guid.Empty;

            await EditingPetValidations.ClearAll();
            await EditPetModal.Show();
        }

        private async Task DeletePetAsync(PetDto input)
        {
            await PetsAppService.DeleteAsync(input.Id);
            await GetPetsAsync();
        }

        private async Task CreatePetAsync()
        {
            try
            {
                if (await NewPetValidations.ValidateAll() == false)
                {
                    return;
                }

                await PetsAppService.CreateAsync(NewPet);
                await GetPetsAsync();
                await CloseCreatePetModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPetModalAsync()
        {
            await EditPetModal.Hide();
        }

        private async Task UpdatePetAsync()
        {
            try
            {
                if (await EditingPetValidations.ValidateAll() == false)
                {
                    return;
                }

                await PetsAppService.UpdateAsync(EditingPetId, EditingPet);
                await GetPetsAsync();
                await EditPetModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }


        private bool IsCreateFormDisabled()
        {
            return OnNewPetImageLoading ;
        }
        
        private bool IsEditFormDisabled()
        {
            return OnEditPetImageLoading ;
        }



        private int MaxPetImageFileUploadSize = 1024 * 1024 * 10; //10MB
        private bool OnNewPetImageLoading = false;
        private async Task OnNewPetImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxPetImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnNewPetImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                NewPet.ImageId = result.Id;
                OnNewPetImageLoading = false;            
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private bool HasSelectedPetImage = false;
        private bool OnEditPetImageLoading = false;
        private async Task OnEditPetImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxPetImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnEditPetImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                EditingPet.ImageId = result.Id;
                OnEditPetImageLoading = false;            
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }            
        }




        private async Task<AppFileDescriptorDto> UploadFileAsync(IBrowserFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(long.MaxValue).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                
                return await PetsAppService.UploadFileAsync(new RemoteStreamContent(ms, file.Name, file.ContentType));
            }
        }



        private async Task DownloadFileAsync(Guid fileId)
        {
            var token = (await PetsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("CoreService") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/pets/file?DownloadToken={token}&FileId={fileId}", forceLoad: true);
        }

        private string GetImageUrl(Guid fileId)
        {
            var remoteService = RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("CoreService").Result ?? RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default").Result;
            return $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/pets/image?ImageId={fileId}";
        }

        protected virtual async Task OnCategoryChangedAsync(Guid? categoryId)
        {
            Filter.CategoryId = categoryId;
            await SearchAsync();
        }
        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnBreedChangedAsync(string? breed)
        {
            Filter.Breed = breed;
            await SearchAsync();
        }
        protected virtual async Task OnAgeMinChangedAsync(float? ageMin)
        {
            Filter.AgeMin = ageMin;
            await SearchAsync();
        }
        protected virtual async Task OnAgeMaxChangedAsync(float? ageMax)
        {
            Filter.AgeMax = ageMax;
            await SearchAsync();
        }
        protected virtual async Task OnGenderChangedAsync(string? gender)
        {
            Filter.Gender = gender;
            await SearchAsync();
        }
        protected virtual async Task OnColorChangedAsync(string? color)
        {
            Filter.Color = color;
            await SearchAsync();
        }
        protected virtual async Task OnWeightMinChangedAsync(float? weightMin)
        {
            Filter.WeightMin = weightMin;
            await SearchAsync();
        }
        protected virtual async Task OnWeightMaxChangedAsync(float? weightMax)
        {
            Filter.WeightMax = weightMax;
            await SearchAsync();
        }
        protected virtual async Task OnHealthStatusChangedAsync(string? healthStatus)
        {
            Filter.HealthStatus = healthStatus;
            await SearchAsync();
        }
        protected virtual async Task OnVaccinationsMinChangedAsync(int? vaccinationsMin)
        {
            Filter.VaccinationsMin = vaccinationsMin;
            await SearchAsync();
        }
        protected virtual async Task OnVaccinationsMaxChangedAsync(int? vaccinationsMax)
        {
            Filter.VaccinationsMax = vaccinationsMax;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMinChangedAsync(decimal? priceMin)
        {
            Filter.PriceMin = priceMin;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMaxChangedAsync(decimal? priceMax)
        {
            Filter.PriceMax = priceMax;
            await SearchAsync();
        }
        protected virtual async Task OnQuantityMinChangedAsync(int? quantityMin)
        {
            Filter.QuantityMin = quantityMin;
            await SearchAsync();
        }
        protected virtual async Task OnQuantityMaxChangedAsync(int? quantityMax)
        {
            Filter.QuantityMax = quantityMax;
            await SearchAsync();
        }
        protected virtual async Task OnIsBoothChangedAsync(bool? isBooth)
        {
            Filter.IsBooth = isBooth;
            await SearchAsync();
        }
        protected virtual async Task OnIsStockChangedAsync(bool? isStock)
        {
            Filter.IsStock = isStock;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllPetsSelected = true;
            
            return Task.CompletedTask;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var result = await CategoriesAppService.GetListAsync(new GetCategoriesInput
                {
                    MaxResultCount = 1000
                });
                CategoryLookupList = result.Items;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private Task ClearSelection()
        {
            AllPetsSelected = false;
            SelectedPets.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedPetRowsChanged()
        {
            if (SelectedPets.Count != PageSize)
            {
                AllPetsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedPetsAsync()
        {
            var message = AllPetsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPets.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllPetsSelected)
            {
                await PetsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await PetsAppService.DeleteByIdsAsync(SelectedPets.Select(x => x.Id).ToList());
            }

            SelectedPets.Clear();
            AllPetsSelected = false;

            await GetPetsAsync();
        }

        // Image Search Methods
        private async Task UploadImage(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                if (file != null)
                {
                    OnImageSearchLoading = true;
                    StateHasChanged();
                    
                    var buffer = new byte[file.Size];
                    await file.OpenReadStream().ReadAsync(buffer);
                    ImageData = buffer;
                    ImagePreview = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
                    
                    OnImageSearchLoading = false;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                OnImageSearchLoading = false;
                await HandleErrorAsync(ex);
            }
        }

        private async Task Predict()
        {
            if (ImageData == null)
            {
                await UiMessageService.Info(L["PleaseUploadImage"]);
                return;
            }

            isPredicting = true;
            StateHasChanged();

            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(ImageData), "image", "upload.jpg");

            try
            {
                var response = await Http.PostAsync("http://127.0.0.1:8080/predict", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(result);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("breed_name", out JsonElement breedNameElement))
                    {
                        PredictionResult = breedNameElement.GetString()?.Trim() ?? "Unknown";
                    }
                    else
                    {
                        PredictionResult = "Unknown";
                    }
                }
                else
                {
                    PredictionResult = "API error";
                }
            }
            catch (Exception ex)
            {
                PredictionResult = "System error";
            }

            isPredicting = false;
            
            // Sử dụng kết quả dự đoán để lọc dữ liệu ngầm
            if (!string.IsNullOrEmpty(PredictionResult))
            {
                Filter.Breed = PredictionResult;
                await GetPetsAsync();
                await UiMessageService.Success(L["SearchCompleted"]);
            }
            
            StateHasChanged();
        }

        private async Task ShowWebcamPopup()
        {
            await WebcamModal.Show();
            await _jsObjectRef!.InvokeVoidAsync("startWebcam");
        }

        private async Task CaptureImage()
        {
            var base64Image = await _jsObjectRef!.InvokeAsync<string>("captureImage");
            ImagePreview = base64Image;
            ImageData = Convert.FromBase64String(base64Image.Split(',')[1]);
            await WebcamModal.Hide();
            await _jsObjectRef!.InvokeVoidAsync("stopWebcam");
        }

        private async Task CloseWebcamPopup()
        {
            await WebcamModal.Hide();
            await _jsObjectRef!.InvokeVoidAsync("stopWebcam");
        }

        // Image modal methods
        private async Task OpenImageModal(Guid fileId)
        {
            enlargedImageUrl = GetImageUrl(fileId);
            showImageModal = true;
            currentZoom = 1.0;
            StateHasChanged();
        }

        private async Task CloseImageModal()
        {
            showImageModal = false;
            StateHasChanged();
        }

        private async Task ZoomIn()
        {
            if (currentZoom < MAX_ZOOM)
            {
                currentZoom += ZOOM_STEP;
                StateHasChanged();
            }
        }

        private async Task ZoomOut()
        {
            if (currentZoom > MIN_ZOOM)
            {
                currentZoom -= ZOOM_STEP;
                StateHasChanged();
            }
        }

        private async Task ResetZoom()
        {
            currentZoom = 1.0;
            StateHasChanged();
        }


    }
}
