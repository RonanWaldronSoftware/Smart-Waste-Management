import React, { useState } from "react";
import LoginLayout from "./components/LoginLayout";
import LoginForm from "./components/LoginForm";
import { FaCheck } from "react-icons/fa";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { forgotPassword } from "../../services/login";

const ForgotPassword = () => {
  const { t } = useTranslation("translation");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);
  const [formSubmitted, setFormSubmitted] = useState(false);
  const [formData, setFormData] = useState({
    email: "",
  });

  const handleChange = (e) => {
    setError(false);
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    /* Validate input values */
    if (formData.email === "") {
      setError(true);
      setLoading(false);
      setErrorMessage(t("emailRequired"));

      return;
    } else {
      try {
        const res = await forgotPassword(formData.email);

        if (res) {
          setFormSubmitted(true);
        } else {
          setError(true);
          setLoading(false);
          setErrorMessage(t("requestError"));
        }
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
      {!formSubmitted ? (
        <LoginForm
          type="forgot-password"
          handleSubmit={handleSubmit}
          handleChange={handleChange}
          formData={formData}
          error={error}
          errorMessage={errorMessage}
          loading={loading}
        />
      ) : (
        <div className="flex flex-col w-full my-auto gap-10">
          <FaCheck size={40} className=" text-white mx-auto" />
          <p className="text-white">{t("requestSubmitted")}</p>
          <Link to="/login" className="btn btn-new">
            {t("login")}
          </Link>
        </div>
      )}
    </LoginLayout>
  );
};

export default ForgotPassword;
