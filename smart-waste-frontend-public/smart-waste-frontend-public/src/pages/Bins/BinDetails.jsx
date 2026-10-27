import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import {
  BinDetailsChart,
  BinDetailsMap,
  Loader,
  MainWrapper,
  NoResults,
  TabList,
} from "../../components";
import {
  Link,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";
import { getTimestamp } from "../../utils/getTimestamp";
import { formatDate } from "../../utils/formatDate";
import { LiaAngleRightSolid } from "react-icons/lia";
import BinReport from "./components/binDetailsPanels/BinReport";
import BinReadings from "./components/binDetailsPanels/BinReadings";
import BinSubscriptions from "./components/binDetailsPanels/BinSubscriptions";
import BinInstalls from "./components/binDetailsPanels/BinInstalls";
import BinDetailsPanel from "./components/binDetailsPanels/BinDetailsPanel";
import {
  AppliancesIcon,
  BinIcon,
  BooksIcon,
  DiscIcon,
  FruitIcon,
  GlassIcon,
  HouseholdIcon,
  LiquidIcon,
  PlasticIcon,
  PrinterIcon,
  RecycleIcon,
  SprayIcon,
  TextileIcon,
  CrockeryIcon,
  BatteryIcon,
  CanIcon,
} from "../../assets";
import { getUnit, resetLocationToDevice } from "../../services/units";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import { FaNewspaper, FaRecycle, FaTrash } from "react-icons/fa";

const BinDetails = () => {
  const accessToken = useSelector((state) => state.user.accessToken);
  const userData = useSelector((state) => state.user.userData);
  const params = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation("translation");
  const [searchParams] = useSearchParams();
  const query = searchParams.get("report");

  const [loading, setLoading] = useState(true);
  const [fetchError, setFetchError] = useState(false);
  const [binData, setBinData] = useState(null);
  const [timestamp, setTimestamp] = useState(null);
  //panels data
  const [detailsPanelData, setDetailsPanelData] = useState(null);
  const [readingsPanelData, setReadingsPanelData] = useState(null);
  const [subscriptionRenewalDate, setSubscriptionRenewalDate] = useState("");
  const [installationData, setInstallationData] = useState(null);

  //chart data
  const [chartData, setChartData] = useState([]);

  const [activeTab, setActiveTab] = useState("details");
  const tabs = [
    {
      id: "details",
      title: t("details"),
    },
    {
      id: "report",
      title: t("damageReport"),
    },
  ];
  const adminTabs = [
    {
      id: "details",
      title: t("details"),
    },
    {
      id: "report",
      title: t("damageReport"),
    },
    {
      id: "readings",
      title: t("devReadings"),
    },
    {
      id: "subscriptions",
      title: t("subscriptions"),
    },
    {
      id: "installs",
      title: t("installs"),
    },
  ];

  //check for search query for report panel
  useEffect(() => {
    if (query && Boolean(query)) {
      setActiveTab("report");
    }
  }, [query]);

  //fetch data
  useEffect(() => {
    fetchData();
  }, []);

  //set panels + chart data and update timestamp
  useEffect(() => {
    if (binData) {
      setDetailsPanelData({
        binAlerts: binData.alertTypes,
        /*  binStatus: {
          binFull: binData.binFull,
          flapStuckOpen: binData.flapStuckOpen,
          batteryOverTempLo: binData.batteryOverTempLO,
          binLocked: binData.binLocked,
          binTilted: binData.binTilted,
          binEmptiedLastPeriod: binData.binEmptiedLastPeriod,
          batteryUvlo: binData.batteryUVLO,
          serviceDoorOpen: binData.serviceDoorOpen,
        }, */
        emptyLevel: binData.emptyLevel,
        fullLevel: binData.fullLevel,
        owner: {
          name: binData.user.name,
          email: binData.user.email,
        },
        binType: binData.unitModelType.name,
        contentType: binData.contentType.name,
        deviceType: binData.unitType.name,
        binLock: binData.binLock,
        serialNumber: binData.serialNumber,
        sensorData: {
          nbIoTSignalStrength: binData.nbIoTSignalStrength,
          readingDateTime: binData.readingDateTime || null,
          rsrq: binData.rsrq,
          binLevelPercent: binData.fillLevelPercent,
          batteryVoltage: binData.batteryVoltage,
          temperature: binData.temperature,
        },
      });

      setReadingsPanelData({
        binLevel: binData.unitReading.binLevel,
        binLevelBc: binData.unitReading.binLevelBc,
        sigfox: binData.sigFoxAltId ? binData.sigFoxAltId : "-",
        batteryVoltage: binData.unitReading.batteryVoltage,
        binLevelStatus: binData.binLevelStatus,
        messageType: binData.unitReading.messageType
          ? binData.unitReading.messageType
          : "-",
        compaction: binData.unitReading.noCompactions
          ? binData.unitReading.noCompactions
          : "-",
        flapOpenings: binData.unitReading.noFlapOpenings
          ? binData.unitReading.noFlapOpenings
          : "-",
        rsrp: binData.unitReading.rsrp ? binData.unitReading.rsrp : "-",
        rsrq: binData.unitReading.rsrq ? binData.unitReading.rsrq : "-",
        rssi: binData.unitReading.rssi ? binData.unitReading.rssi : "-",
        snr: binData.unitReading.snr ? binData.unitReading.snr : "-",
        src: binData.unitReading.src ? binData.unitReading.src : "-",
        binJustOn: binData.unitReading.unitJustOn ? t("yes") : t("no"),
        firmware: binData.unitReading.firmware
          ? binData.unitReading.firmware
          : "-",
        nbIot: binData.unitReading.nbloTissue ? t("yes") : t("no"),
        periodicReport: binData.unitReading.regularPeriodicReporting
          ? t("yes")
          : t("no"),
        timeDifference: binData.unitReading.timeDiff
          ? binData.unitReading.timeDiff
          : "-",
      });

      setChartData((prev) => [
        {
          id: "binData",
          value: binData.fillLevelPercentage,
          color:
            binData.fillLevelPercentage < binData.emptyLevel
              ? "#2ABC2A"
              : binData.fillLevelPercentage >= binData.emptyLevel &&
                binData.fillLevelPercentage < binData.fullLevel
              ? "#D8C11D"
              : "#B70202",
        },
        {
          id: "constant",
          value: 100 - binData.fillLevelPercentage,
          color: "#333",
        },
      ]);

      setSubscriptionRenewalDate(binData.subscriptionRenewalDate);

      const fetchingTime = getTimestamp();
      setTimestamp(fetchingTime);
      setLoading(false);
    }
  }, [binData]);

  const fetchData = async () => {
    try {
      const unitData = await fetchWithRetry(
        getUnit,
        accessToken,
        navigate,
        t,
        params.id
      );
      // Check if the returned value is null, indicating a logout
      if (unitData.status === 200) {
        // Continue with the regular data handling
        setBinData(unitData.data);
        setInstallationData({
          date: unitData.data.installationDate || null,
          image: unitData.data.unitImages[0].image || null,
        });
      } else {
        setFetchError(true);
        setLoading(false);
      }
    } catch (error) {
      setLoading(false);
      console.error(error);
    }
  };

  // Restore the unit position to the last position reported by the device (admin only)
  const handleResetLocationToDevice = async () => {
    try {
      // fetchWithRetry handles the token refresh and toasts any error message
      const result = await fetchWithRetry(
        resetLocationToDevice,
        accessToken,
        navigate,
        t,
        params.id
      );

      if (result?.status === 200) {
        await fetchData();
      }
    } catch (error) {
      console.error(error);
    }
  };

  const handleTabChange = (tabId) => {
    setActiveTab(tabId);
  };

  if (loading) {
    return (
      <MainWrapper loader={true}>
        <Loader />
      </MainWrapper>
    );
  }

  if (fetchError) {
    return (
      <MainWrapper>
        <div className="flex flex-col w-full">
          <NoResults />
        </div>
      </MainWrapper>
    );
  }

  return (
    <MainWrapper>
      <div className="flex flex-col w-full">
        {/* Header */}
        <div className="flex flex-wrap justify-between items-center bg-white py-3 px-6 mb-6">
          {/* Breadcrumbs */}
          <ul className="flex items-center gap-3 text-xs">
            <li>
              <Link
                to="/bins"
                className=" text-themeColors-accentColor hover:text-themeColors-accentColorDark"
              >
                {t("bins")}
              </Link>
            </li>
            <li>
              <LiaAngleRightSolid />
            </li>
            <li> {binData.location} </li>
          </ul>

          <div className="flex items-center gap-3">
            {/* <button
              className="btn btn-primary disabled:opacity-70 disabled:pointer-events-none"
              onClick={() => pullReadingsData(params.id, binData.serialNumber)}
              disabled={pullingReadings}
            >
              {t("pullReadings")}
            </button> */}
            {userData && userData?.role?.id !== 5 && (
              <Link
                className="btn btn-primary"
                to={`/bins/edit-bin/${params.id}`}
              >
                {t("edit")}
              </Link>
            )}
          </div>
        </div>

        {/* Content */}
        <div className="px-6 flex flex-col lg:flex-row gap-4">
          {/* Graph */}
          <div className="card w-full h-fit lg:w-1/2 xl:w-1/3 bg-white relative pt-12">
            {/* Tag */}
            <div className="flex flex-col absolute left-0 top-1 py-3 px-5">
              {binData.contentType.id === 1 ? (
                <FaTrash size={30} />
              ) : binData.contentType.id === 2 ? (
                <FaRecycle size={30} />
              ) : binData.contentType.id === 5 ? (
                <DiscIcon width={40} />
              ) : binData.contentType.id === 6 ? (
                <BooksIcon width={40} />
              ) : binData.contentType.id === 7 ? (
                <FruitIcon width={40} />
              ) : binData.contentType.id === 8 ? (
                <LiquidIcon width={40} />
              ) : binData.contentType.id === 9 ? (
                <HouseholdIcon width={40} />
              ) : binData.contentType.id === 10 ? (
                <SprayIcon width={40} />
              ) : binData.contentType.id === 11 ? (
                <AppliancesIcon width={40} />
              ) : binData.contentType.id === 12 ? (
                <TextileIcon width={40} />
              ) : binData.contentType.id === 13 ? (
                <PlasticIcon width={17} />
              ) : binData.contentType.id === 14 ? (
                <GlassIcon width={17} />
              ) : binData.contentType.id === 15 ? (
                <PrinterIcon width={40} />
              ) : binData.contentType.id === 16 ? (
                <CrockeryIcon width={40} />
              ) : binData.contentType.id === 17 ? (
                <BatteryIcon />
              ) : binData.contentType.id === 18 ? (
                <CanIcon />
              ) : binData.contentType.id === 19 ? (
                <FaNewspaper size={30} />
              ) : (
                ""
              )}
              {/* {binData.contentType.id === 1 ? <BinIcon /> : <RecycleIcon />} */}
              <p>{binData.contentType.name}</p>
            </div>

            {/* Graph */}
            <div className="relative pb-10 p-5 " style={{ height: "230px" }}>
              <BinDetailsChart data={chartData} />
              <p className="text-center -mt-10 text-2xl font-medium text-themeColors-bgColor">
                {binData.fillLevelPercentage} %
              </p>
            </div>

            {/* Info items */}
            <div className="grid grid-cols-2 gap-4 mb-9 px-5">
              {/* Item */}
              <p className=" bg-[#f6f6f6] flex items-center p-4 gap-4 rounded text-black text-lg">
                <svg
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                  style={{ minWidth: "24px" }}
                >
                  <path
                    fillRule="evenodd"
                    clipRule="evenodd"
                    d="M12 2C13.66 2 15 3.34 15 5V13C16.21 13.91 17 15.37 17 17C17 19.76 14.76 22 12 22C9.24 22 7 19.76 7 17C7 15.37 7.79 13.91 9 13V5C9 3.34 10.34 2 12 2ZM12 4C11.45 4 11 4.45 11 5V11H13V9H12V8H13V6H12V5H13C13 4.45 12.55 4 12 4Z"
                    fill="black"
                    fillOpacity="0.54"
                  ></path>
                </svg>
                <span className=" text-themeColors-bgColor font-medium">
                  {binData.temperature ? binData.temperature + " °C" : "-"}
                </span>
              </p>
              <p className=" bg-[#f6f6f6] flex items-center p-4 gap-4 rounded text-black text-lg">
                <svg
                  style={{ minWidth: "24px" }}
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M15.67 4H14V2H10V4H8.33C7.6 4 7 4.6 7 5.33V20.66C7 21.4 7.6 22 8.33 22H15.66C16.4 22 17 21.4 17 20.67V5.33C17 4.6 16.4 4 15.67 4Z"
                    fill="black"
                    fillOpacity="0.54"
                  ></path>
                </svg>
                <span className=" text-themeColors-bgColor font-medium">
                  {binData.batteryVoltage
                    ? binData.batteryVoltage.toFixed(2) + " V"
                    : "-"}
                </span>
              </p>
              <p className=" bg-[#f6f6f6] flex items-center p-4 gap-4 rounded text-black text-lg col-span-2">
                <svg
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M2 6H4V18H2V6ZM5 6H6V18H5V6ZM7 6H10V18H7V6ZM11 6H12V18H11V6ZM14 6H16V18H14V6ZM17 6H20V18H17V6ZM21 6H22V18H21V6Z"
                    fill="#666666"
                  ></path>
                </svg>
                <span className=" text-themeColors-bgColor font-medium">
                  {binData.serialNumber}{" "}
                </span>
              </p>
              <p className=" bg-[#f6f6f6] flex items-center p-4 gap-4 rounded text-black text-lg col-span-2">
                <svg
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    fillRule="evenodd"
                    clipRule="evenodd"
                    d="M5 9C5 5.13 8.13 2 12 2C15.87 2 19 5.13 19 9C19 14.25 12 22 12 22C12 22 5 14.25 5 9ZM9.5 9C9.5 10.38 10.62 11.5 12 11.5C13.38 11.5 14.5 10.38 14.5 9C14.5 7.62 13.38 6.5 12 6.5C10.62 6.5 9.5 7.62 9.5 9Z"
                    fill="black"
                    fillOpacity="0.54"
                  ></path>
                </svg>
                <span className=" text-themeColors-bgColor font-medium">
                  {binData.location}{" "}
                </span>
              </p>
            </div>

            {/* Manual position */}
            {binData?.isManualLocation && (
              <div className="flex flex-wrap items-center gap-3 mb-4 px-5">
                <span className="rounded bg-amber-100 px-2 py-1 text-xs text-amber-800">
                  {t("manualPositionBadge")}
                </span>

                {userData?.role?.id === 1 && (
                  <button
                    type="button"
                    disabled={
                      !binData?.deviceLatitude || !binData?.deviceLongitude
                    }
                    title={
                      binData?.deviceLatitude
                        ? t("useDevicePositionTooltip")
                        : t("noDevicePosition")
                    }
                    onClick={handleResetLocationToDevice}
                    className="rounded border px-3 py-1 text-sm disabled:opacity-50"
                  >
                    {binData?.deviceLocationDate
                      ? t("useDevicePositionReported", {
                          reported: formatDate(binData.deviceLocationDate),
                        })
                      : t("useDevicePosition")}
                  </button>
                )}
              </div>
            )}

            {/* Map */}
            <BinDetailsMap
              binLocation={{
                lat: binData.latitude,
                lng: binData.longitude,
              }}
              wasteType={binData.contentType.id}
              binCapacity={
                binData.fillLevelPercentage < binData.emptyLevel
                  ? "low"
                  : binData.fillLevelPercentage >= binData.emptyLevel &&
                    binData.fillLevelPercentage < binData.fullLevel
                  ? "medium"
                  : "high"
              }
            />
          </div>

          {/* Panels */}
          <div className="card w-full bg-white">
            <TabList
              tabs={
                userData?.role?.id === 1 || userData?.role?.id === 2
                  ? adminTabs
                  : tabs
              }
              activeTab={activeTab}
              handleTabChange={handleTabChange}
            />

            {/* Panels */}
            {activeTab === "details" && (
              <BinDetailsPanel
                binId={params.id}
                timestamp={timestamp}
                detailsPanelData={detailsPanelData}
                canLockBin={!!userData && userData?.role?.id !== 5}
              />
            )}
            {activeTab === "report" && <BinReport unitId={params.id} />}
            {activeTab === "readings" && (
              <BinReadings readingsPanelData={readingsPanelData} />
            )}
            {activeTab === "subscriptions" && (
              <BinSubscriptions
                subscriptionRenewalDate={subscriptionRenewalDate}
              />
            )}
            {activeTab === "installs" && (
              <BinInstalls installationData={installationData} />
            )}
          </div>
        </div>
      </div>
    </MainWrapper>
  );
};

export default BinDetails;
