import React, { useEffect, useState } from "react";
import { BiCheck, BiWindowOpen } from "react-icons/bi";
import {
  FaBatteryQuarter,
  FaDoorOpen,
  FaExclamationCircle,
  FaFileExport,
  FaLock,
  FaTrashAlt,
  FaTrashRestoreAlt,
} from "react-icons/fa";
import { GoHistory } from "react-icons/go";
import { Link, useNavigate } from "react-router-dom";
import { handleBinLock } from "../../../../services/units";
import { toast } from "react-toastify";
import { useDispatch, useSelector } from "react-redux";
import { useTranslation } from "react-i18next";
import AlertModal from "../AlertModal";
import SensorsChart from "../../../../components/charts/SensorsChart";
import { logout } from "../../../../store/userSlice.js";
import { getUnitReadings } from "../../../../services/units";
import { fetchWithRetry } from "../../../../utils/refreshAccessToken.js";
import LoaderSmall from "../../../../components/LoaderSmall.jsx";
import BinDamaged from "../../../../assets/icons/BinDamaged";

const BinDetailsPanel = ({ binId, timestamp, detailsPanelData, canLockBin }) => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const accessToken = useSelector((state) => state.user.accessToken);
  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(true);
  const [sensorGraphData, setSensorGraphData] = useState(null);
  const [sensorGraphFilters, setSensorGraphFilters] = useState([
    "fillLevel",
    "temperature",
    "battery",
    "signal",
  ]);

  const [alertModalActive, setAlertModalActive] = useState(false);
  const [alertModalData, setAlertModalData] = useState(null);

  const [lockPeriod, setLockPeriod] = useState("1"); // Selected lock period value (1-24 or 'custom')
  const [endDate, setEndDate] = useState(""); // Selected end date for custom period
  const [endTime, setEndTime] = useState(""); // Selected end time for custom period
  const [binLocked, setBinLocked] = useState(detailsPanelData?.binLock);
  const [lockSubmitting, setLockSubmitting] = useState(false);

  // Helper function to get the last 7 days
  const getLast7Days = () => {
    const today = new Date();
    const dates = [];
    for (let i = 6; i >= 0; i--) {
      const date = new Date(today);
      date.setDate(today.getDate() - i);
      dates.push(formatDateToCustom(date));
    }
    return dates;
  };
  //format date to dd.mm.
  function formatDateToCustom(inputDate) {
    const date = new Date(inputDate);

    const day = date.getDate().toString().padStart(2, "0"); // Get day as "23"
    const month = (date.getMonth() + 1).toString().padStart(2, "0"); // Get month as "08"

    return `${day}.${month}.`;
  }

  useEffect(() => {
    fetchSensorData();
  }, []);

  useEffect(() => {
    if (detailsPanelData) {
      setBinLocked(detailsPanelData.binLock);
    }
  }, [detailsPanelData]);

  const fetchSensorData = async () => {
    try {
      const data = await fetchWithRetry(
        getUnitReadings,
        accessToken,
        navigate,
        t,
        binId,
        6,
        "",
        1,
        1000
      );
      if (data.status === 200) {
        const refactoredDataArr = data.data.items.map((reading) => ({
          signalLevel: !reading.rsrq
            ? 0
            : reading.rsrq >= -7
            ? 100
            : reading.rsrq >= -11
            ? 50
            : 10,
          batteryLevel: reading.batteryVoltage
            ? reading.batteryVoltage.toFixed(2)
            : 0,
          fillLevel: reading.binLevel ? reading.binLevel : 0,
          temperature: reading.temperature ? reading.temperature : 0,
          readingDate: formatDateToCustom(reading.readingDate),
        }));

        /*  const filtered = refactoredDataArr.filter(
          (item) => item.batteryLevel > 0 && item.temperature > 0
        ); */

        // Create a map for quick lookup
        const dataMap = refactoredDataArr.reduce((map, reading) => {
          if (!map[reading.readingDate]) {
            map[reading.readingDate] = [];
          }
          map[reading.readingDate].push(reading);
          return map;
        }, {});

        // Get the last 7 days and fill missing dates with default values
        const last7Days = getLast7Days();
        // Prepare complete data array
        const completeDataArr = [];
        last7Days.forEach((date) => {
          if (dataMap[date]) {
            dataMap[date].forEach((reading) => {
              completeDataArr.push({ ...reading, readingDate: date });
            });
          } else {
            // If no readings for the date, add an empty object with all values 0
            completeDataArr.push({
              readingDate: date,
              signalLevel: 0,
              batteryLevel: 0,
              fillLevel: 0,
              temperature: 0,
            });
          }
        });

        setSensorGraphData(completeDataArr);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };
  const handleLockPeriodChange = (e) => {
    setLockPeriod(e.target.value);
    // Reset custom date values when a lock period is selected
    setEndDate("");
    setEndTime("");
  };
  /* 
  const formatDateTime = (date) => {
    //return new Date(date).toISOString(); // Formats date in the desired format
    const year = date.getUTCFullYear();
    const month = String(date.getUTCMonth() + 1).padStart(2, "0");
    const day = String(date.getUTCDate()).padStart(2, "0");
    const hours = String(date.getUTCHours()).padStart(2, "0");
    const minutes = String(date.getUTCMinutes()).padStart(2, "0");
    const seconds = String(date.getUTCSeconds()).padStart(2, "0");
    const timezoneOffset = date.getTimezoneOffset();
    const offsetHours = Math.floor(Math.abs(timezoneOffset) / 60)
      .toString()
      .padStart(2, "0");
    const offsetMinutes = (Math.abs(timezoneOffset) % 60)
      .toString()
      .padStart(2, "0");
    const offsetSign = timezoneOffset < 0 ? "-" : "+";

    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}.000${offsetSign}${offsetHours}${offsetMinutes}`;
  };
 */
  const handleLocking = async () => {
    setLockSubmitting(true);
    let unlockTime = "";
    if (lockPeriod === "custom" && endDate && endTime) {
      const datetimeString = `${endDate}T${endTime}:00`;
      const date = new Date(datetimeString);
      unlockTime = date.toISOString();
    } else if (lockPeriod) {
      const currentTime = new Date();
      const hours = parseInt(lockPeriod, 10);
      const unlockDateTime = new Date(
        currentTime.getTime() + hours * 60 * 60 * 1000
      );
      unlockTime = unlockDateTime.toISOString();
    }

    try {
      const res = await handleBinLock(
        accessToken,
        binId,
        unlockTime,
        !binLocked
      );
      if (res.status === 200) {
        toast.success(t(binLocked ? "binUnlocked" : "binLocked"));
        setBinLocked((prev) => !prev);
      }
    } catch (error) {
      toast.error(t("requestError"));
      console.error(error);
    } finally {
      setLockSubmitting(false);
    }
  };

  const handleAlertModal = (alertId) => {
    if (alertModalActive) {
      setAlertModalActive(false);
      setAlertModalData(null);
    } else {
      setAlertModalActive(true);
      getModalData(alertId);
    }
  };
  const getModalData = (alertId) => {
    let label = "";
    let desc = "";
    let action = "";
    switch (alertId) {
      case 1:
        label = t("batteryUVLO");
        desc = t("batteryUVLODesc");
        action = t("batteryUVLOAction");
        break;
      case 2:
        label = t("binEmptiedLastPeriod");
        desc = t("binEmptiedLastPeriodDesc");
        action = t("binEmptiedLastPeriodAction");
        break;
      case 3:
        label = t("batteryOverTempLO");
        desc = t("batteryOverTempLODesc");
        action = t("batteryOverTempLOAction");
        break;
      case 4:
        label = t("binLocked");
        desc = t("binLockedDesc");
        action = t("binLockedDescAction");
        break;
      case 5:
        label = t("binTilted");
        desc = t("binTiltedDesc");
        action = t("binTiltedAction");
        break;
      case 6:
        label = t("serviceDoorOpen");
        desc = t("serviceDoorOpenDesc");
        action = t("serviceDoorOpenAction");
        break;
      case 7:
        label = t("flapStuckOpen");
        desc = t("flapStuckOpenDesc");
        action = t("flapStuckOpenAction");
        break;
      case 8:
        label = t("binFull");
        desc = t("binFullDesc");
        action = t("binFullAction");
        break;
      case 9:
        label = "Damage report";
        desc = "";
        action = "";
        break;
      case 10:
        label = "Device not reporting";
        desc = "";
        action = "";
        break;
      // Add more cases for other error names if needed
      default:
        break;
    }
    setAlertModalData({ label, desc, action });
  };

  const handleGraphFilters = (filter) => {
    if (sensorGraphFilters.includes(filter)) {
      const filtered = sensorGraphFilters.filter((item) => item !== filter);
      setSensorGraphFilters(filtered);
    } else {
      setSensorGraphFilters((prev) => [...prev, filter]);
    }
  };

  return (
    <section className=" p-12 pt-5">
      {/* Graph */}
      {sensorGraphData && sensorGraphData.length > 0 && (
        <div className="flex flex-col gap-6">
          <div className="flex items-center justify-between py-6">
            <h2 className=" text-2xl font-medium">{t("sensors")}</h2>
          </div>
          {/* Filters */}
          <div className="flex justify-center items-center gap-3">
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                className="sr-only peer"
                name="fillLevel"
                checked={sensorGraphFilters.includes("fillLevel")}
                onChange={() => handleGraphFilters("fillLevel")}
              />

              <div className="w-11 h-6 relative bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[1px] after:left-[2px] after:bg-themeColors-dangerColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-successColor peer-checked:border-themeColors-successColor peer-checked:after:bg-white"></div>
              <span>{t("fillLevel")}</span>
            </label>
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                className="sr-only peer"
                name="temperature"
                checked={sensorGraphFilters.includes("temperature")}
                onChange={() => handleGraphFilters("temperature")}
              />

              <div className="w-11 h-6 relative bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[1px] after:left-[2px] after:bg-themeColors-dangerColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-black peer-checked:border-black peer-checked:after:bg-white"></div>
              <span>{t("temp")}</span>
            </label>
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                className="sr-only peer"
                name="battery"
                checked={sensorGraphFilters.includes("battery")}
                onChange={() => handleGraphFilters("battery")}
              />

              <div className="w-11 h-6 relative bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[1px] after:left-[2px] after:bg-themeColors-dangerColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-dangerColor peer-checked:border-themeColors-dangerColor peer-checked:after:bg-white"></div>
              <span>{t("battery")}</span>
            </label>
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                className="sr-only peer"
                name="signal"
                checked={sensorGraphFilters.includes("signal")}
                onChange={() => handleGraphFilters("signal")}
              />

              <div className="w-11 h-6 relative bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[1px] after:left-[2px] after:bg-themeColors-dangerColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-dangerColor peer-checked:bg-opacity-80 peer-checked:border-themeColors-dangerColor peer-checked:after:bg-white"></div>
              <span>{t("signal")}</span>
            </label>
          </div>

          <SensorsChart data={sensorGraphData} filters={sensorGraphFilters} />
        </div>
      )}
      {/* Panel header */}
      <div className="flex items-center justify-between py-6">
        <h2 className=" text-2xl font-medium">{t("detailsAndReadings")}</h2>
        <Link
          to={`/bins/${binId}/history`}
          className="btn btn-secondary flex items-center gap-4"
        >
          <GoHistory />
          {t("allReadings")}
        </Link>
      </div>

      {/* Panel content */}
      <div className="flex flex-col">
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("binStatus")}</p>
          {/* Tags */}
          <div className="w-full flex flex-wrap gap-1">
            {detailsPanelData?.binAlerts?.length > 0 ? (
              detailsPanelData?.binAlerts?.map((alert) => (
                <span
                  key={alert.id}
                  onClick={() => handleAlertModal(alert.id)}
                  className="cursor-pointer flex items-center gap-3 w-fit bg-themeColors-warningColor text-white font-medium rounded py-1 px-2 text-xs"
                >
                  {alert.id === 1 ? (
                    <FaBatteryQuarter />
                  ) : alert.id === 2 ? (
                    <FaExclamationCircle />
                  ) : alert.id === 3 ? (
                    <FaBatteryQuarter className=" rotate-12" />
                  ) : alert.id === 4 ? (
                    <FaLock />
                  ) : alert.id === 5 ? (
                    <FaTrashAlt className="rotate-15" />
                  ) : alert.id === 6 ? (
                    <FaDoorOpen />
                  ) : alert.id === 7 ? (
                    <BiWindowOpen />
                  ) : alert.id === 8 ? (
                    <FaTrashRestoreAlt />
                  ) : alert.id === 9 ? (
                    <BinDamaged />
                  ) : alert.id === 10 ? (
                    <FaFileExport />
                  ) : (
                    <></>
                  )}
                  {alert.name}
                </span>
              ))
            ) : (
              <span className="flex items-center gap-3 w-fit bg-themeColors-successColor text-white font-medium rounded py-1 px-2 text-xs">
                <BiCheck />
                {t("flagSuccess")}
              </span>
            )}
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("dataRefreshed")}</p>
          <div className="w-full">
            <p className="text-black">{timestamp}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("emptyLevel")}</p>
          <div className="w-full">
            <p className="text-black">{detailsPanelData?.emptyLevel}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("fullLevel")}</p>
          <div className="w-full">
            <p className="text-black">{detailsPanelData?.fullLevel}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("owner")}</p>
          <div className="w-full">
            <p className="text-black">{`${detailsPanelData?.owner.name}(${detailsPanelData?.owner.email})`}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("binType")}</p>
          <div className="w-full">
            <p className="text-black">{detailsPanelData?.binType}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("contentType")}</p>
          <div className="w-full">
            <p className="text-black">{detailsPanelData?.contentType}</p>
          </div>
        </div>
        {/* Row */}
        <div className="flex items-center gap-6 py-4 border-b border-solid border-b-[#e9ecef]">
          <p className=" w-1/2 lg:w-1/4 text-[#757575]">{t("deviceType")}</p>
          <div className="w-full">
            <p className="text-black">{detailsPanelData?.deviceType}</p>
          </div>
        </div>
        {/* Row */}
        {canLockBin && !detailsPanelData?.serialNumber?.startsWith("086") && (
          <div className="flex items-center gap-6 py-4">
            <p className=" w-1/2 lg:w-1/4 text-[#757575] mb-auto">
              {t(binLocked ? "unlockBin" : "lockBin")}
            </p>
            <div className="w-full flex flex-col gap-4">
              <select
                name="lockPeriod"
                id="lockPeriod"
                className="input"
                value={lockPeriod}
                onChange={handleLockPeriodChange}
              >
                {[
                  1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
                  19, 20, 21, 22, 23, 24,
                ].map((hours) => (
                  <option key={hours} value={hours}>
                    {hours} {hours === 1 ? t("hour") : t("hours")}
                  </option>
                ))}
                <option value="custom">{t("custom")}</option>
              </select>

              {/* Date inputs */}
              {lockPeriod === "custom" && (
                <div className="flex flex-col gap-1">
                  <label htmlFor="lockPeriodDate">
                    {t(binLocked ? "chooseUnlockPeriod" : "chooseLockPeriod")}
                  </label>
                  <div className="flex gap-2">
                    <input
                      type="date"
                      name="lockPeriodDate"
                      id="lockPeriodDate"
                      className="input"
                      value={endDate}
                      onChange={(e) => setEndDate(e.target.value)}
                    />
                    <input
                      type="time"
                      name="lockPeriodTime"
                      id="lockPeriodTime"
                      className="input"
                      value={endTime}
                      onChange={(e) => setEndTime(e.target.value)}
                    />
                  </div>
                </div>
              )}

              <button
                className="btn btn-primary"
                onClick={handleLocking}
                disabled={lockSubmitting}
              >
                {lockSubmitting ? (
                  <LoaderSmall />
                ) : (
                  <>{t(binLocked ? "unlockBin" : "lock")}</>
                )}
              </button>
            </div>
          </div>
        )}
      </div>

      {alertModalActive && (
        <AlertModal handleModal={handleAlertModal} data={alertModalData} />
      )}
    </section>
  );
};

export default BinDetailsPanel;
