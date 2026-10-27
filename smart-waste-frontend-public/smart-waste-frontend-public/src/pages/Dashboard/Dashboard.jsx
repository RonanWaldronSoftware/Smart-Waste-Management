import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import {
  DashboardChart,
  Loader,
  MainWrapper,
  NoResults,
  TabList,
} from "../../components";
import DashboardCards from "./components/DashboardCards";
import GraphCards from "./components/GraphCards";
import { useNavigate } from "react-router-dom";
import { getTimestamp } from "../../utils/getTimestamp";
import { useTranslation } from "react-i18next";
import { getDashboardData } from "../../services/lookup";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import LoaderSmall from "../../components/LoaderSmall";

const Dashboard = () => {
  const navigate = useNavigate();
  const accessToken = useSelector((state) => state.user.accessToken);

  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(true);
  const [refetch, setRefetch] = useState(false);
  const [fetchError, setFetchError] = useState(false);
  const [timestamp, setTimestamp] = useState("");

  const [dashboardData, setDashboardData] = useState(null);
  const [mixBinsCount, setMixBinsCount] = useState(null);
  const [recycleBinsCount, setRecycleBinsCount] = useState(null);
  const [warningsCount, setWarningsCount] = useState(null);
  const [fullBins, setFullBins] = useState(null);

  const [binsChartValues, setBinsChartValues] = useState(null);
  const [chartData, setChartData] = useState(null);

  const [activeTab, setActiveTab] = useState("all");
  const [activeIndex, setActiveIndex] = useState(0);

  //fetch bins data
  useEffect(() => {
    fetchData();
    // Fetch data every 1 minute
    const intervalId = setInterval(() => {
      setRefetch(true);
      fetchData();
    }, 60000);

    // Clear the interval when the component unmounts
    return () => clearInterval(intervalId);
  }, []);

  //update chart data every time active tab changes
  useEffect(() => {
    setBinsChartValues(countBinsByName(dashboardData, activeTab));
  }, [activeTab, dashboardData]);

  //set chart data
  useEffect(() => {
    if (binsChartValues) {
      setChartData([
        {
          name: t("low"),
          value: binsChartValues[0],
          color: "#2ABC2A",
        },
        {
          name: t("medium"),
          value: binsChartValues[1],
          color: "#D8C11D",
        },
        {
          name: t("high"),
          value: binsChartValues[2],
          color: "#B70202",
        },
      ]);
    }
  }, [binsChartValues]);

  // Only set loading to false when both binsChartValues and chartData are available
  useEffect(() => {
    if (dashboardData && binsChartValues && chartData) {
      setLoading(false);
    }
  }, [binsChartValues, chartData, dashboardData]);

  //update timestamp
  useEffect(() => {
    // Update the timestamp whenever dashboardData changes
    if (dashboardData) {
      const fetchingTime = getTimestamp();
      setTimestamp(fetchingTime);
    }
  }, [dashboardData]);

  //function for fetching data
  const fetchData = async () => {
    try {
      const data = await fetchWithRetry(
        getDashboardData,
        accessToken,
        navigate,
        t
      );
      if (data?.status === 200) {
        setDashboardData(data.data);
        setMixBinsCount(
          data.data.contentTypesCount.find((item) => item.contentTypeId === 1)
            .count
        );
        setRecycleBinsCount(
          data.data.contentTypesCount.find((item) => item.contentTypeId === 2)
            .count
        );
        setWarningsCount(data.data.unitsWithAlertsCount);
        const mixedWasteData = data.data.contentFullnessLevel.find(
          (item) => item.contentTypeId === 1
        );
        const recycledWasteData = data.data.contentFullnessLevel.find(
          (item) => item.contentTypeId === 2
        );
        setFullBins({
          mixed: {
            count: mixedWasteData.criticalFullnessCount,
            liters: mixedWasteData.criticalFullnessInLiters,
          },
          recycle: {
            count: recycledWasteData.criticalFullnessCount,
            liters: recycledWasteData.criticalFullnessInLiters,
          },
        });
        // Perform the logout and redirect
      } else {
        setFetchError(true);
        setLoading(false);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setRefetch(false);
    }
  };

  //function for generating chart values array based on passed name
  const countBinsByName = (latestReadings, name) => {
    if (!latestReadings) {
      return [];
    }
    const mixedWasteData = dashboardData.contentFullnessLevel.find(
      (item) => item.contentTypeId === 1
    );
    const recycledWasteData = dashboardData.contentFullnessLevel.find(
      (item) => item.contentTypeId === 2
    );

    if (name === "mix") {
      return [
        mixedWasteData.smallFullnessCount,
        mixedWasteData.mediumFullnessCount,
        mixedWasteData.criticalFullnessCount,
      ];
    }

    if (name === "rec") {
      return [
        recycledWasteData.smallFullnessCount,
        recycledWasteData.mediumFullnessCount,
        recycledWasteData.criticalFullnessCount,
      ];
    }

    return [
      mixedWasteData.smallFullnessCount + recycledWasteData.smallFullnessCount,
      mixedWasteData.mediumFullnessCount +
        recycledWasteData.mediumFullnessCount,
      mixedWasteData.criticalFullnessCount +
        recycledWasteData.criticalFullnessCount,
    ];
  };

  const handleTabChange = (tabId) => {
    setActiveTab(tabId);
  };

  //function for hover effect on chart
  const onPieEnter = (_, index) => {
    setActiveIndex(index);
  };
  //function for hover effect on chart cards
  const handleExternalPieHover = (index) => {
    setActiveIndex(index);
  };

  const handleMapRedirect = (filterValue) => {
    navigate(`/map?filter=${filterValue}`);
  };

  const tabs = [
    {
      id: "all",
      title: t("all"),
    },
    {
      id: "mix",
      title: t("mixed"),
    },
    {
      id: "rec",
      title: t("recycle"),
    },
  ];

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
        {/* Title */}
        <div className="flex flex-wrap bg-white py-3 px-6 mb-6">
          <h1 className="w-auto self-center text-2xl font-medium leading-tight text-themeColors-textColor flex items-centerg gap-4">
            {t("dashboard")}
            {refetch && <LoaderSmall />}
          </h1>
        </div>

        {/* Content */}
        <div className="px-6 flex flex-col gap-4">
          {/* Cards */}
          <DashboardCards
            recycleBinsCount={recycleBinsCount}
            warningsCount={warningsCount}
            mixBinsCount={mixBinsCount}
            fullBins={fullBins}
            handleRedirect={handleMapRedirect}
          />

          {/* Graph container */}
          <div className="card">
            {/* Tabs */}
            <TabList
              tabs={tabs}
              activeTab={activeTab}
              handleTabChange={handleTabChange}
            />

            {/* Panel */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
              <div className="p-5">
                {/* Status */}
                <div className="flex flex-col gap-2">
                  <h4 className=" text-xl font-medium leading-tight">
                    {t("currentStatus")}
                  </h4>
                  <p className=" text-sm font-light">
                    {t("dataRefreshed")} {timestamp}
                  </p>
                </div>

                {/* Graph */}
                {/* <div className="w-full h-full">Graph here</div> */}
                {chartData && (
                  <DashboardChart
                    data={chartData}
                    activeIndex={activeIndex}
                    onPieEnter={onPieEnter}
                  />
                )}
              </div>

              <GraphCards handleExternalPieHover={handleExternalPieHover} />
            </div>
          </div>
        </div>
      </div>
    </MainWrapper>
  );
};

export default Dashboard;
