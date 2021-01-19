﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElsaDashboard.Application.Models;
using ElsaDashboard.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ElsaDashboard.Application.Shared
{
    partial class FlyoutPanel
    {
        [Inject] private IFlyoutPanelService FlyoutPanelService { get; set; } = default!;
        [Parameter] public string? Title { get; set; }
        [Parameter] public RenderFragment Content { get; set; } = default!;
        [Parameter] public ICollection<ButtonDescriptor> Buttons { get; set; } = new List<ButtonDescriptor>();

        protected override void OnInitialized()
        {
            FlyoutPanelService.OnShow += Update;
        }

        private async void Update()
        {
            var options = FlyoutPanelService.Options;
            
            await InvokeAsync(() =>
            {
                Content = builder =>
                {
                    var i = 0;
                    builder.OpenComponent(i++, options.ContentComponentType);
                    
                    foreach (var (name, value) in options.Parameters)
                        builder.AddAttribute(i++, name, value);

                    if(options.ContentComponentReference != null)
                        builder.AddComponentReferenceCapture(i++, options.ContentComponentReference);
                    
                    builder.CloseComponent();
                };

                Buttons = options.Buttons;
                Title = options.Title;
                StateHasChanged();
            });
        }

        private async ValueTask OnButtonClickAsync(ButtonDescriptor button)
        {
            if(button.ClickHandler != null)
                await button.ClickHandler.Value.InvokeAsync(new ButtonClickEventArgs(button));
        }
    }
}