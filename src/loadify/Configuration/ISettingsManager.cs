﻿namespace loadify.Configuration
{
    public interface ISettingsManager
    {
        IDirectorySetting DirectorySetting { get; set; }
        IBehaviorSetting BehaviorSetting { get; set; }
        ICredentialsSetting CredentialsSetting { get; set; }
        ILocalizationSetting LocalizationSetting { get; set; }
        IInternalSetting InternalSetting { get; set; }
    }
}
