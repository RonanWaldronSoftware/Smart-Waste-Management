import { Route, Routes, useLocation, useNavigate } from "react-router-dom";
import { Aside, Loader, MainWrapper, NoResults, Topbar } from "./components";
import {
  Dashboard,
  ForgotPassword,
  Login,
  Map,
  ResetPassword,
  BinsList,
  BinDetails,
  CreateBin,
  Alerts,
  Reports,
  Users,
  EditBin,
  CreateBulkBin,
  RoutesPage,
  EditRoute,
  AddRoute,
} from "./pages";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { logout, setConfigData, setUserData } from "./store/userSlice";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import BinHistoryReadings from "./pages/Bins/BinHistoryReadings";
import { getUser } from "./services/users";
import ForgotPasswordReset from "./pages/Login/ForgotPasswordReset";
import Privacy from "./pages/Privacy";
import { getConfig } from "./services/lookup";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "./utils/refreshAccessToken";

function App() {
  const accessToken = localStorage.getItem("smartWasteAccessToken");
  const userData = useSelector((state) => state.user.userData);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useTranslation();
  const [loading, setLoading] = useState(true);
  const [firstFetch, setFirstFetch] = useState(true);
  const [fetchError, setFetchError] = useState(false);

  useEffect(() => {
    setFetchError(false);
    if (!accessToken) {
      if (
        !location.pathname.includes("/forgot-password") &&
        !location.pathname.includes("/privacy") &&
        !location.pathname.includes("/login")
      ) {
        navigate("/login");
        dispatch(logout());
        setLoading(false);
      }
    } else {
      fetchData();
      fetchConfig();
      setLoading(false);
    }
  }, [location]);

  const fetchData = async () => {
    if (!userData) {
      console.log("fetching initial data");
      try {
        const data = await fetchWithRetry(getUser, accessToken, navigate, t);
        if (data?.status === 200) {
          dispatch(setUserData(data.data));
        } else {
          setFetchError(true);
        }
      } catch (error) {
        console.error(error);
      }
    }
  };
  const fetchConfig = async () => {
    try {
      const data = await fetchWithRetry(getConfig, accessToken, navigate, t);
      if (data?.status === 200) {
        dispatch(setConfigData(data.data));
        setFirstFetch(false);
      } else {
        setFetchError(true);
      }
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <>
      {!location.pathname.includes("/login") &&
        !location.pathname.includes("/forgot-password") &&
        !location.pathname.includes("/privacy") && <Topbar />}
      {!location.pathname.includes("/login") &&
        !location.pathname.includes("/forgot-password") &&
        !location.pathname.includes("/privacy") && <Aside />}
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/login/reset-password" element={<ResetPassword />} />
        <Route path="/login/forgot-password" element={<ForgotPassword />} />
        <Route
          path="/forgot-password/:code"
          element={<ForgotPasswordReset />}
        />

        <Route
          path="/"
          element={
            loading ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                      <p>App error</p>
                    </div>
                  </MainWrapper>
                ) : (
                  <Dashboard />
                )}
              </>
            )
          }
        />
        <Route
          path="/map"
          element={
            loading ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                    </div>
                  </MainWrapper>
                ) : (
                  <Map />
                )}
              </>
            )
          }
        />
        <Route
          path="/bins"
          element={
            loading ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                    </div>
                  </MainWrapper>
                ) : (
                  <BinsList />
                )}
              </>
            )
          }
        />
        <Route
          path="/bins/:id"
          element={
            loading ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                    </div>
                  </MainWrapper>
                ) : (
                  <BinDetails />
                )}
              </>
            )
          }
        />
        <Route
          path="/bins/:id/history"
          element={
            loading ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                    </div>
                  </MainWrapper>
                ) : (
                  <BinHistoryReadings />
                )}
              </>
            )
          }
        />
        <Route
          path="/bins/create-bin"
          element={
            userData && userData?.role?.id === 1 ? (
              <>
                {loading ? (
                  <MainWrapper loader={true}>
                    <Loader />
                  </MainWrapper>
                ) : (
                  <>
                    {fetchError ? (
                      <MainWrapper>
                        <div className="flex flex-col w-full">
                          <NoResults />
                          <h1 className="text-xl font-bold text-center">
                            {t("requestError")}
                          </h1>
                        </div>
                      </MainWrapper>
                    ) : (
                      <CreateBin />
                    )}
                  </>
                )}
              </>
            ) : (
              <MainWrapper>
                <div className="flex flex-col w-full items-center mt-10">
                  <h1 className=" text-3xl text-themeColors-dangerColor">
                    You don't have access to this site
                  </h1>
                  <NoResults />
                </div>
              </MainWrapper>
            )
          }
        />
        <Route
          path="/bins/create-bulk-bin"
          element={
            userData && userData?.role?.id === 1 ? (
              <>
                {loading ? (
                  <MainWrapper loader={true}>
                    <Loader />
                  </MainWrapper>
                ) : (
                  <>
                    {fetchError ? (
                      <MainWrapper>
                        <div className="flex flex-col w-full">
                          <NoResults />
                          <h1 className="text-xl font-bold text-center">
                            {t("requestError")}
                          </h1>
                        </div>
                      </MainWrapper>
                    ) : (
                      <CreateBulkBin />
                    )}
                  </>
                )}
              </>
            ) : (
              <MainWrapper>
                <div className="flex flex-col w-full items-center mt-10">
                  <h1 className=" text-3xl text-themeColors-dangerColor">
                    You don't have access to this site
                  </h1>
                  <NoResults />
                </div>
              </MainWrapper>
            )
          }
        />
        <Route
          path="/bins/edit-bin/:id"
          element={
            loading || !userData ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : userData?.role?.id !== 5 ? (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                    </div>
                  </MainWrapper>
                ) : (
                  <EditBin />
                )}
              </>
            ) : (
              <MainWrapper>
                <div className="flex flex-col w-full items-center mt-10">
                  <h1 className=" text-3xl text-themeColors-dangerColor">
                    You don't have access to this site
                  </h1>
                  <NoResults />
                </div>
              </MainWrapper>
            )
          }
        />
        <Route
          path="/alerts"
          element={
            loading ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                    </div>
                  </MainWrapper>
                ) : (
                  <Alerts />
                )}
              </>
            )
          }
        />
        <Route
          path="/reports"
          element={
            loading ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : (
              <>
                {fetchError ? (
                  <MainWrapper>
                    <div className="flex flex-col w-full">
                      <NoResults />
                      <h1 className="text-xl font-bold text-center">
                        {t("requestError")}
                      </h1>
                    </div>
                  </MainWrapper>
                ) : (
                  <Reports />
                )}
              </>
            )
          }
        />
        <Route
          path="/users"
          element={
            userData && userData?.role?.id >= 1 && userData?.role?.id <= 4 ? (
              <>
                {loading ? (
                  <MainWrapper loader={true}>
                    <Loader />
                  </MainWrapper>
                ) : (
                  <>
                    {fetchError ? (
                      <MainWrapper>
                        <div className="flex flex-col w-full">
                          <NoResults />
                          <h1 className="text-xl font-bold text-center">
                            {t("requestError")}
                          </h1>
                        </div>
                      </MainWrapper>
                    ) : (
                      <Users />
                    )}
                  </>
                )}
              </>
            ) : (
              <MainWrapper>
                <div className="flex flex-col w-full items-center mt-10">
                  <h1 className=" text-3xl text-themeColors-dangerColor">
                    You don't have access to this site
                  </h1>
                  <NoResults />
                </div>
              </MainWrapper>
            )
          }
        />
        {/* <Route path="/system" element={<System />} /> */}
        <Route
          path="/routes"
          element={
            fetchError ? (
              <MainWrapper>
                <div className="flex flex-col w-full">
                  <NoResults />
                  <h1 className="text-xl font-bold text-center">
                    {t("requestError")}
                  </h1>
                </div>
              </MainWrapper>
            ) : (
              <RoutesPage />
            )
          }
        />
        <Route
          path="/routes/add"
          element={
            loading || !userData ? (
              <MainWrapper loader={true}>
                <Loader />
              </MainWrapper>
            ) : userData?.role?.id >= 1 && userData?.role?.id <= 4 ? (
              fetchError ? (
                <MainWrapper>
                  <div className="flex flex-col w-full">
                    <NoResults />
                    <h1 className="text-xl font-bold text-center">
                      {t("requestError")}
                    </h1>
                  </div>
                </MainWrapper>
              ) : (
                <AddRoute />
              )
            ) : (
              <MainWrapper>
                <div className="flex flex-col w-full items-center mt-10">
                  <h1 className=" text-3xl text-themeColors-dangerColor">
                    You don't have access to this site
                  </h1>
                  <NoResults />
                </div>
              </MainWrapper>
            )
          }
        />
        <Route
          path="/routes/:id"
          element={
            fetchError ? (
              <MainWrapper>
                <div className="flex flex-col w-full">
                  <NoResults />
                  <h1 className="text-xl font-bold text-center">
                    {t("requestError")}
                  </h1>
                </div>
              </MainWrapper>
            ) : (
              <EditRoute />
            )
          }
        />
        {/*  <Route path="/routes/edit/:id" element={<EditRoute />} /> */}
        <Route path="/privacy-policy" element={<Privacy />} />
      </Routes>
      <ToastContainer autoClose={10000} />
    </>
  );
}

export default App;
