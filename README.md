# Mini Factory
## Запуск проекта
- **Редактор:** Unity 6.3.22f1
- **Сцена:** `Assets/_MiniFactory/Scenes/SampleScene.unity` — открыть и нажать Play
- **Готовая сборка:** `apk` лежит в корне проекта
## Основные системы и архитектурные решения
### Архитектура и слои
Проект разделён на сборки (asmdef) по слоям.

Ключевые решения:
- **MVP**: View пассивный (только сеттеры `SetXxx`), Presenter форматирует данные и вызывает сервисы, Model хранит чистое состояние.
- **DI (VContainer)**: все зависимости создаются и регистрируются в `GameLifetimeScope`, `GameBootstrap` резолвит их и связывает Presenter'ы.
- **Изоляция от SDK**: gameplay не знает о конкретных SDK — IAP и аналитика подключены через интерфейсы `IIapService` и `IAnalyticsProvider`.
- **События несут только изменённое значение**, остальное доступно через свойства модели.

### Экономика и машины
- `MachineModel` — состояние машины (locked/unlocked, уровень). Инварианты: locked ⇒ уровень 0, unlocked ⇒ уровень ≥ 1.
- Формулы (`MachineCalculator`): производство `baseProduction * productionGrowth^(level-1)`, стоимость апгрейда `baseUpgradeCost * upgradeGrowth^(level-1)`.
- `EconomyModel` — баланс и суммарное производство; `EconomyService.Tick(dt)` начисляет доход `totalProduction * activeMultiplier * dt`.

### Boost
- `BoostModel` — покупка и активация буста, таймер, множитель (`ActiveMultiplier`).
- `BoostService` — покупка за валюту, активация, обратный отсчёт.
- Множитель применяется только при начислении дохода и в отображении — модели хранят базовое производство (без двойного применения).

### Оффлайн-прогресс
- `OfflineProgressCalculator` считает доход за время отсутствия с учётом лимита (`offlineCapSeconds`) и активного буста (сначала расходуется остаток буста).
- `OfflineService.Apply(elapsed)` начисляет доход и применяет расход буста.

### Сохранение и жизненный цикл
- `IStorageService` / `FileStorageService` — запись и чтение JSON (`JsonUtility`).
- `SaveService` хранит `SaveData`: баланс, состояние машин, буст и время последнего сохранения.
- `GameBootstrap` автосохраняет раз в 30 секунд и по событиям `AppLifecycleObserver` (Paused / Resumed / Quitting); при возврате применяет оффлайн-прогресс.

### Конфигурация
- `IGameConfigProvider` + `LocalConfigProvider`; данные лежат в ScriptableObject (`FactoryConfigAsset`, `MachineConfigAsset`) и конвертируются в чистые Core-DTO (`GameConfig`, `MachineConfig`).
- Провайдер заменяем на удалённый (например, Firebase Remote Config) без изменений в gameplay.

### IAP
- `IIapService` — абстракция; `UnityIapService` реализует её поверх Unity IAP v5 (store services, fake store в редакторе).
- `PurchaseService` связывает успешную покупку с начислением валюты; продукт `coins_pack_small` = 10 000 монет.

### Аналитика
- `IAnalyticsProvider` — интерфейс провайдера; `AnalyticsService` рассылает событие во все подключённые провайдеры (ошибка одного не ломает остальные).
- Обязательный `DebugAnalyticsProvider` пишет события в консоль.
- 8 событий: `game_started`, `machine_unlocked`, `machine_upgraded`, `boost_started`, `boost_finished`, `offline_income_applied`, `purchase_succeeded`, `purchase_failed`.

### Тесты
Были написаны unit тесты на основные модели: 
- Boost
- Economy
- Machine
- NumberFormatter
- OfflineProgress
## Использованные packages/SDK

| Пакет                                  | Версия | Назначение                                                           |
| -------------------------------------- | ------ | -------------------------------------------------------------------- |
| `jp.hadashikick.vcontainer`            | 1.19.0 | DI-контейнер (VContainer) — сборка графа зависимостей                |
| `com.unity.purchasing`                 | 5.4.4  | Unity IAP v5 — внутриигровые покупки (consumable `coins_pack_small`) |
| `com.unity.ugui`                       | 2.0.0  | uGUI + TextMeshPro — интерфейс                                       |
| `com.unity.test-framework`             | 1.6.0  | NUnit-тесты (EditMode)                                               |
| `com.coplaydev.unity-mcp`              | git    | MCP-мост к редактору (инструмент разработки, в билд не входит)       |

Остальные пакеты (`com.unity.2d.*`, `com.unity.ide.*`, `com.unity.timeline`, `com.unity.visualscripting` и модули движка) — стандартный набор шаблона Unity 2D, отдельно не использовались.
## Известные ограничения / незавершенные части
- **Конфигурация:** реализован только локальный провайдер (ScriptableObject); Firebase Remote Config не подключён.
- **Аналитика:** только обязательный Debug-провайдер; реальный SDK (Firebase/Unity Analytics) не подключён.
- **IAP:** проверено только на Fake Store в редакторе; реальные сторы и серверная валидация чеков не тестировались.
- **Сохранение:** локальный JSON-файл без облачной синхронизации и защиты от подмены.
- **Оффлайн-прогресс:** считается по системным часам (перевод времени устройства увеличивает доход).
- **Тесты:** только EditMode на модели и калькуляторы; PlayMode/интеграционных тестов нет.
- **UI/контент:** простой интерфейс без анимаций и иконок машин; баланс чисел тестовый, не сбалансирован.

## Устройство/эмулятор, на котором проверялся build
Redmi Note 10 Pro
## Фактическое время работы
По времени работы ориентировался на этапы, составленные AI агентом. 
Основные задачи были выполнены примерно за 4:40 на mvp приложения + 15м на билд и профайлинг + 20 минут на readme. Из дополнительных были сделаны тесты и профайлинг.

## Profiling
Собрал development build и подключил профайлер. Сделал снимок кадра и дал агенту для анализа. Билд ограничен на 30 кадров.
Результаты:
- Ран из 2000 кадров, стабильные **~33.3 мс ≈ 30 FPS** (min 29.5 / max 37.6 мс), кадров >50 мс нет.
- Время кадра уходит в **ожидание презентации (GPU/vSync)**, а не в CPU: `Gfx.WaitForPresentOnGfxThread` ~15.1 мс + `WaitForLastPresentationAndUpdateTime` ~9.0 мс.
- Реальная отрисовка URP ~5.7 мс (`DoRenderLoop_Internal`); вся игровая логика (`ScriptRunBehaviourUpdate`) <0.4 мс; физика ~0.07 мс.
- Render Thread блокируется в `GfxDeviceVK.Present` ~26 мс (Vulkan present ждёт vSync/GPU).
- Причина — включённый vSync на текущем уровне качества (`QualitySettings.m_CurrentQuality: 5`, `vSyncCount: 1`).
- Итог: узкое место — путь вывода кадра (GPU/vSync), не скрипты и не физика. 30 FPS стабильны; для 60 нужно отключить vSync/targetFrameRate и замерить GPU.
