using System.Collections.Generic;
using MiniFactory.Config.Local;
using MiniFactory.Core;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Config;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Iap;
using MiniFactory.Core.Machines;
using MiniFactory.Game.Services;
using MiniFactory.Iap.UnityIAP;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MiniFactory.App
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private FactoryConfigAsset _configAsset;

        protected override void Configure(IContainerBuilder builder)
        {
            LocalConfigProvider configProvider = new LocalConfigProvider(_configAsset);
            builder.RegisterInstance<IGameConfigProvider>(configProvider);

            GameConfig config = null;
            configProvider.Load(loaded => config = loaded);

            builder.RegisterInstance(config ?? new GameConfig());

            EconomyModel economyModel = new EconomyModel(BuildMachines(config));
            BoostModel boostModel = new BoostModel(config);
            EconomyService economyService = new EconomyService(economyModel, boostModel);
            BoostService boostService = new BoostService(boostModel, economyModel);
            OfflineService offlineService = new OfflineService(economyModel, boostModel, config);
            UnityIapService iapService = new UnityIapService(BuildProductIds(config));
            PurchaseService purchaseService = new PurchaseService(iapService, economyModel, config);

            builder.RegisterInstance(economyModel);
            builder.RegisterInstance(boostModel);
            builder.RegisterInstance(economyService);
            builder.RegisterInstance(boostService);
            builder.RegisterInstance(offlineService);
            builder.RegisterInstance<IIapService>(iapService);
            builder.RegisterInstance(purchaseService);
            builder.Register<FileStorageService>(Lifetime.Singleton).As<IStorageService>();
            builder.Register<SaveService>(Lifetime.Singleton);
        }

        private static List<string> BuildProductIds(GameConfig config)
        {
            List<string> productIds = new List<string>();

            if (config == null || config.products == null)
            {
                return productIds;
            }

            for (int i = 0; i < config.products.Length; i++)
            {
                ProductConfig product = config.products[i];

                if (product != null && !string.IsNullOrEmpty(product.id))
                {
                    productIds.Add(product.id);
                }
            }

            return productIds;
        }

        private static IEnumerable<MachineModel> BuildMachines(GameConfig config)
        {
            List<MachineModel> machines = new List<MachineModel>();

            if (config == null || config.machines == null)
            {
                return machines;
            }

            for (int i = 0; i < config.machines.Length; i++)
            {
                MachineConfig machineConfig = config.machines[i];

                if (machineConfig == null)
                {
                    continue;
                }

                machines.Add(machineConfig.RequiresUnlock
                    ? new MachineModel(machineConfig)
                    : new MachineModel(machineConfig, MachineState.Unlocked, 1));
            }

            return machines;
        }
    }
}
