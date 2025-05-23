﻿using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for periodic polling.
    /// </summary>
    public class Acquisitor : IDisposable
	{
		private AutoResetEvent acquisitionTrigger;
        private IProcessingManager processingManager;
        private Thread acquisitionWorker;
		private IStateUpdater stateUpdater;
		private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Acquisitor"/> class.
        /// </summary>
        /// <param name="acquisitionTrigger">The acquisition trigger.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="stateUpdater">The state updater.</param>
        /// <param name="configuration">The configuration.</param>
		public Acquisitor(AutoResetEvent acquisitionTrigger, IProcessingManager processingManager, IStateUpdater stateUpdater, IConfiguration configuration)
		{
			this.stateUpdater = stateUpdater;
			this.acquisitionTrigger = acquisitionTrigger;
			this.processingManager = processingManager;
			this.configuration = configuration;
			this.InitializeAcquisitionThread();
			this.StartAcquisitionThread();
		}

		#region Private Methods

        /// <summary>
        /// Initializes the acquisition thread.
        /// </summary>
		private void InitializeAcquisitionThread()
		{
			this.acquisitionWorker = new Thread(Acquisition_DoWork);
			this.acquisitionWorker.Name = "Acquisition thread";
		}

        /// <summary>
        /// Starts the acquisition thread.
        /// </summary>
		private void StartAcquisitionThread()
		{
			acquisitionWorker.Start();
		}

        /// <summary>
        /// Acquisitor thread logic.
        /// </summary>
		private void Acquisition_DoWork()
		{
            //TO DO: IMPLEMENT

            //poslednja kolona(10) u rtu nam govori na kolko se sekundi citaju vrednosti registara

            List<IConfigItem> configItems = this.configuration.GetConfigurationItems();
            while(true)
            {
                acquisitionTrigger.WaitOne();
                foreach(IConfigItem item in configItems)
                {
                    item.SecondsPassedSinceLastPoll++;

                    if (item.SecondsPassedSinceLastPoll == item.AcquisitionInterval)
                    { //ako se poklapa sa onim intervalom koji je postavljen u fajlu
                      //iz itema uzimamo informacije koje citamo na nekoliko sekundi
                        processingManager.ExecuteReadCommand(item, this.configuration.GetTransactionId(), configuration.UnitAddress, item.StartAddress, item.NumberOfRegisters);

                        item.SecondsPassedSinceLastPoll = 0;    //restartujemo brojac za citanje
                    }
                }



            }

        }

        #endregion Private Methods

        /// <inheritdoc />
        public void Dispose()
		{
			acquisitionWorker.Abort();
        }
	}
}