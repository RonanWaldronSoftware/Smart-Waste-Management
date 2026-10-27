import React, { useEffect, useState } from "react";
import LoginLayout from "./components/LoginLayout";
import LoginForm from "./components/LoginForm";
import { useDispatch } from "react-redux";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import {
  checkToken,
  fetchLoginData,
  resetPassword,
} from "../../services/login";
import { useTranslation } from "react-i18next";
import { setAccessToken, setIsLoggedIn } from "../../store/userSlice";
import { Loader } from "../../components";
import { logo } from "../../assets";

const ResetPassword = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [checking, setChecking] = useState(true);
  const [tokenError, setTokenError] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);
  const [formData, setFormData] = useState({
    password: "",
    confirmPassword: "",
  });
  const oneTimeToken = localStorage.getItem("oneTimeToken");
  const { t } = useTranslation("translation");

  const handleChange = (e) => {
    setError(false);
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  useEffect(() => {
    checkOneTimeToken();
  }, []);

  const checkOneTimeToken = async () => {
    try {
      const res = await checkToken(oneTimeToken);
      if (res.status !== 200) {
        setError(true);
        setErrorMessage(
          "Your link is broken, please submit a new reset password request"
        );
        setTokenError(true);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setChecking(false);
    }
  };
  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    /* Validate input values */
    if (formData.password === "" || formData.confirmPassword === "") {
      setError(true);
      setLoading(false);
      setErrorMessage(t("passRequired"));
      return;
    } else if (formData.password !== formData.confirmPassword) {
      setError(true);
      setLoading(false);
      setErrorMessage(t("passMatch"));
      return;
    } else if (formData.password.length < 5) {
      setError(true);
      setLoading(false);
      setError(t("passError"));
      return;
    } else {
      try {
        const res = await resetPassword(oneTimeToken, formData.password);
        if (res.status === 200) {
          toast.success(t("passwordSuccessfullyUpdated"), {
            position: "top-right",
            autoClose: 3000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: false,
            progress: undefined,
            theme: "light",
          });
          const loginData = {
            email: localStorage.getItem("email"),
            password: formData.password,
          };
          const fetchedData = await fetchLoginData(loginData);
          if (fetchedData.status === 200) {
            localStorage.removeItem("oneTimeToken");
            localStorage.removeItem("email");
            localStorage.setItem(
              "smartWasteAccessToken",
              fetchedData.data.jwtAuthResult.accessToken
            );
            localStorage.setItem(
              "smartWasteRefreshToken",
              JSON.stringify(fetchedData.data.jwtAuthResult.refreshToken)
            );
            dispatch(
              setAccessToken(fetchedData.data.jwtAuthResult.accessToken)
            );
            dispatch(setIsLoggedIn(true));
            navigate("/");
          }
        } else {
          setError(true);
          setErrorMessage(t("requestError"));
        }
      } catch (error) {
        setError(true);
        setErrorMessage(t("requestError"));
        console.error(error);
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <LoginLayout>
      {checking ? (
        <div className="flex flex-col w-full my-auto">
          {/* Error message */}
          {error && <p className="text-center  px-14 pt-4 ">{errorMessage}</p>}

          {/* Form content */}
          <div className="flex flex-col px-14 pt-10 gap-10">
            <Loader />
          </div>
        </div>
      ) : (
        <LoginForm
          type="reset-password"
          handleSubmit={handleSubmit}
          handleChange={handleChange}
          formData={formData}
          error={error}
          errorMessage={errorMessage}
          loading={loading}
          tokenError={tokenError}
        />
      )}
    </LoginLayout>
  );
};

export default ResetPassword;
