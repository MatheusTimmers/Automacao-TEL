using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DataBaseClass;
using MainSpecAn.Assays;
using MainSpecAn.Session;
using Windows.Storage;

namespace MainSpecAn
{
    /// <summary>
    /// Orchestrates test execution.
    /// Receives a TestSession (instrument + output folder + alias), runs the correct
    /// assay class and persists results to CSV and database.
    /// </summary>
    public class AssayRunner
    {
        private readonly TestSession _session;

        // Maps display names (shown in UI) to DB table names (internal, stable).
        // Keeps UI labels decoupled from the database schema.
        private static readonly Dictionary<string, string> TableNameMap =
            new Dictionary<string, string>
            {
                ["Largura de Faixa a 6 dB"]                          = "OccupiedBandwidth6dB",
                ["Largura de Faixa a 26 dB"]                         = "OccupiedBandwidth26dB",
                ["Largura de Faixa a 20 db"]                         = "OccupiedBandwidth20dB",
                ["Potência de Pico Máxima"]                          = "MaximumPeakPower",
                ["Valor Médio da Potência máxima de Saida"]          = "AverageMaximumOutputPower",
                ["Pico da Densidade de Potência"]                    = "PeakPowerSpectralDensity",
                ["Valor Médio da Densidade Espectral de Potência"]   = "AveragePowerSpectralDensity",
                ["Emissão fora de faixa"]                            = "OutOfBandEmissions",
                ["Emissão Fora da Faixa"]                            = "OutOfBandEmissions",
                ["Potência de Saída"]                                = "OutputPower",
                ["Densidade Espectral de Potência"]                  = "PowerSpectralDensity",
                ["Separação de Canais de Salto"]                     = "HoppingChannelSeparation",
                ["Numero de Ocupações"]                              = "NumberOfOccupations",
                ["Tempo de Ocupação"]                                = "OccupationTime",
            };

        public AssayRunner(TestSession session)
        {
            _session = session;
        }

        // ── WiFi assays ────────────────────────────────────────────────────────

        public async Task RunWifiAssayAsync(
            string user,
            string modu,
            string valFreq,
            string nameAssay,
            string bandwidth,
            string refLevel,
            string att,
            bool   tPrints,
            IProgress<string> progress = null)
        {
            progress?.Report($"Starting: {nameAssay} @ {valFreq} MHz");

            var instrument = _session.Instrument
                ?? throw new InvalidOperationException("Instrument not connected.");

            AssayResult result;

            switch (nameAssay)
            {
                case "Largura de Faixa a 6 dB":
                    result = await new AssayOccupiedBandwidth(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, "6", tPrints);
                    break;

                case "Largura de Faixa a 26 dB":
                    result = await new AssayOccupiedBandwidth(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, "26", tPrints);
                    break;

                case "Potência de Pico Máxima":
                    result = await new AssayMaximumPeakPower(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Valor Médio da Potência máxima de Saida":
                    result = await new AssayAverageMaximumOutputPower(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Pico da Densidade de Potência":
                    result = await new AssayPeakPowerSpectralDensity(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Valor Médio da Densidade Espectral de Potência":
                    result = await new AssayAveragePowerSpectralDensity(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Potência de Saída":
                    result = await new AssayOutputPower(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Densidade Espectral de Potência":
                    result = await new AssayPowerSpectralDensity(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Emissão fora de faixa":
                    // TODO: implementar AssayEmissaoForaDeFaixa
                    progress?.Report("Emissão fora de faixa: não implementado.");
                    return;

                default:
                    return;
            }

            await PersistResultAsync(result, "WIFI", user, modu, valFreq, nameAssay, "N9010A");
            progress?.Report($"Done: {nameAssay} @ {valFreq} MHz");
        }

        // ── Bluetooth assays ───────────────────────────────────────────────────

        public async Task RunBTAssayAsync(
            string user,
            string modu,
            string valFreq,
            string nameAssay,
            string bandwidth,
            string refLevel,
            string att,
            bool   tPrints,
            IProgress<string> progress = null)
        {
            progress?.Report($"Starting: {nameAssay} @ {valFreq} MHz");

            var instrument = _session.Instrument
                ?? throw new InvalidOperationException("Instrument not connected.");

            AssayResult result;

            switch (nameAssay)
            {
                case "Largura de Faixa a 20 db":
                    result = await new AssayOccupiedBandwidth(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, "20", tPrints);
                    break;

                case "Potência de Pico Máxima":
                    result = await new AssayMaximumPeakPower(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Pico da Densidade de Potência":
                    result = await new AssayPeakPowerSpectralDensity(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Numero de Ocupações":
                    result = await new AssayAverageMaximumOutputPower(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Tempo de Ocupação":
                    result = await new AssayAveragePowerSpectralDensity(instrument)
                        .ExecuteAsync(valFreq, bandwidth, refLevel, att, tPrints);
                    break;

                case "Emissão Fora da Faixa":
                    // TODO: implementar AssayEmissaoForaDeFaixa
                    progress?.Report("Emissão Fora da Faixa: não implementado.");
                    return;

                default:
                    return;
            }

            await PersistResultAsync(result, "BT", user, modu, valFreq, nameAssay, "N9010A");
            progress?.Report($"Done: {nameAssay} @ {valFreq} MHz");
        }

        // ── Persistence ────────────────────────────────────────────────────────

        private async Task PersistResultAsync(
            AssayResult result,
            string tech,
            string user,
            string modu,
            string valFreq,
            string displayName,
            string model)
        {
            string tableName = TableNameMap.TryGetValue(displayName, out var t) ? t : displayName;
            StorageFolder csvFolder = await CreateOutputFolderAsync(tech, modu, displayName);

            if (result.Value.HasValue)
            {
                string val = result.Value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                await SaveValueCsvAsync(csvFolder, displayName, valFreq, val);
                SaveDb(modu, valFreq, val, tableName, model, user);
            }

            if (result.Screenshot != null)
                await SaveImageAsync(displayName, valFreq, result.Screenshot, csvFolder);
        }

        private async Task<StorageFolder> CreateOutputFolderAsync(string tech, string modu, string displayName)
        {
            if (_session.OutputFolder == null)
                throw new InvalidOperationException("Output folder not selected.");

            StorageFolder root = await _session.OutputFolder
                .CreateFolderAsync(_session.Alias ?? "Assay", CreationCollisionOption.OpenIfExists);

            string techFolder = tech == "WIFI" ? "Wifi" : "Bluetooth";
            root = await root.CreateFolderAsync(techFolder, CreationCollisionOption.OpenIfExists);
            root = await root.CreateFolderAsync(modu,       CreationCollisionOption.OpenIfExists);

            StorageFile csvFile = await root.CreateFileAsync(
                displayName + ".csv", CreationCollisionOption.OpenIfExists);
            string existing = await FileIO.ReadTextAsync(csvFile);
            if (string.IsNullOrEmpty(existing))
                await FileIO.WriteTextAsync(csvFile, $"Assay:;{displayName};Modulation:;{modu}\n");

            return root;
        }

        private static async Task SaveValueCsvAsync(
            StorageFolder folder, string displayName, string valFreq, string val)
        {
            StorageFile csvFile = await folder.CreateFileAsync(
                displayName + ".csv", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(csvFile, $"Freq:;{valFreq};Value:;{val}\n");
        }

        private static async Task SaveImageAsync(
            string displayName, string valFreq, byte[] image, StorageFolder folder)
        {
            StorageFile file = await folder.CreateFileAsync(
                $"{valFreq} {displayName}.png", CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteBytesAsync(file, image);
        }

        private static void SaveDb(
            string modu, string valFreq, string val,
            string tableName, string model, string user)
        {
            if (!DataBaseCommands.IsValidTableName(tableName))
                throw new ArgumentException($"Invalid table name: {tableName}");

            var cmd = new DataBaseCommands();
            cmd.ClearParameters();
            cmd.AddParameters("@modu",    modu);
            cmd.AddParameters("@ValFreq", valFreq);
            cmd.AddParameters("@Value",   val);
            cmd.AddParameters("@Model",   model);
            cmd.AddParameters("@User",    user);
            cmd.ExecuteCommand(CommandType.Text,
                $"INSERT INTO {tableName} (Modu, ValFreq, Value, Model, User) " +
                $"VALUES (@modu, @ValFreq, @Value, @Model, @User)");
        }
    }
}
