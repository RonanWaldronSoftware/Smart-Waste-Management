import React, { useEffect, useState } from "react";
import LoginLayout from "./components/LoginLayout";
import LoginForm from "./components/LoginForm";
import { useNavigate, useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { useTranslation } from "react-i18next";
import { toast } from "react-toastify";
import { checkToken, resetPassword } from "../../services/login";
import { logo } from "../../assets";
import { Loader } from "../../components";
import { setAccessToken, setIsLoggedIn } from "../../store/userSlice";

const ForgotPasswordReset = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [extractingQueries, setExtractingQueries] = useState(true);
  const [secretKey, setSecretKey] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);
  const [formData, setFormData] = useState({
    password: "",
    confirmPassword: "",
  });
  const [checking, setChecking] = useState(true);
  const [tokenError, setTokenError] = useState(false);
  const { t } = useTranslation("translation");

  const params = useParams();
  useEffect(() => {
    if (params.code) {
      setSecretKey(params.code);
    } else {
      toast.error(
        "Your reset link is broken, please submit a new reset password request"
      );
    }
    setExtractingQueries(false);
  }, [params]);

  useEffect(() => {
    if (secretKey) {
      checkOneTimeToken();
    }
  }, [secretKey]);

  const checkOneTimeToken = async () => {
    try {
      const res = await checkToken(secretKey);
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

  const handleChange = (e) => {
    setError(false);
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
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
        const res = await resetPassword(secretKey, formData.password);
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
          dispatch(setAccessToken(res.data.accessToken));
          localStorage.setItem("smartWasteAccessToken", res.data.accessToken);
          localStorage.setItem(
            "smartWasteRefreshToken",
            JSON.stringify(res.data.refreshToken)
          );
          /* dispatch(setUserId(parsedUserData.id)); */

          dispatch(setIsLoggedIn(true));

          navigate("/");
        }
        setLoading(false);
      } catch (error) {
        setError(true);
        setLoading(false);
        setErrorMessage(t("requestError"));
        console.error(error);
      }
    }
  };
  return (
    <LoginLayout>
      {checking ? (
        <div className="flex flex-col w-full my-auto">
          {/* Error message */}
          {error && <p className="text-center pt-4 ">{errorMessage}</p>}

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

export default ForgotPasswordReset;
