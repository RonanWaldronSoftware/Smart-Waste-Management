import React, { useEffect, useState } from "react";
import { FaCheck, FaExclamationTriangle, FaWrench } from "react-icons/fa";
import { useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import {
  getPossibleAssignedUsers,
  getUnitDamages,
  updateDamages,
} from "../../../../services/damage";
import { Loader, NoResults } from "../../../../components";
import { useTranslation } from "react-i18next";
import { BiMinus, BiPlus } from "react-icons/bi";
import { fetchWithRetry } from "../../../../utils/refreshAccessToken";
import { toast } from "react-toastify";
import { formatDate } from "../../../../utils/formatDate";
import LoaderSmall from "../../../../components/LoaderSmall";

const BinReport = ({ unitId }) => {
  const navigate = useNavigate();
  const params = useParams();
  const accessToken = useSelector((state) => state.user.accessToken);
  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(true);
  const [damageData, setDamageData] = useState(null);
  const [userNames, setUserNames] = useState(null);

  const [formData, setFormData] = useState({
    statusId: 4,
    comment: "",
    assignedUserId: "",
  });
  const [formSubmitting, setFormSubmitting] = useState(false);
  //accordion states
  const [activeAccordion, setActiveAccordion] = useState(null);

  const handleAccordion = (accId) => {
    if (accId === activeAccordion) {
      setActiveAccordion(null);
    } else {
      setActiveAccordion(accId);
    }
  };

  useEffect(() => {
    fetchData();
    fetchOwners();
  }, []);

  const fetchData = async () => {
    try {
      const data = await fetchWithRetry(
        getUnitDamages,
        accessToken,
        navigate,
        t,
        params.id
      );
      if (data.status === 200) {
        setDamageData(data.data);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };
  const fetchOwners = async () => {
    try {
      const data = await fetchWithRetry(
        getPossibleAssignedUsers,
        accessToken,
        navigate,
        t,
        unitId
      );
      if (data.status === 200) {
        setUserNames(data.data);
      }
    } catch (error) {
      setLoading(false);
      console.error(error);
    }
  };

  const handleStatus = () => {
    if (formData.statusId !== 6) {
      setFormData((prev) => ({ ...prev, statusId: 6 }));
    } else {
      setFormData((prev) => ({ ...prev, statusId: 4 }));
    }
  };
  const handleSubmit = async () => {
    setFormSubmitting(true);
    const apiData = {
      damageId: activeAccordion,
      statusId:
        formData.statusId === 6
          ? formData.statusId
          : formData.assignedUserId !== ""
          ? 5
          : 4,
      comment: formData.comment,
      assignedUserId:
        formData.assignedUserId !== "" ? Number(formData.assignedUserId) : null,
    };

    try {
      const data = await fetchWithRetry(
        updateDamages,
        accessToken,
        navigate,
        t,
        apiData
      );

      if (data.status === 200) {
        toast.success("Damage updated");
        fetchData();
      }
    } catch (error) {
      console.error(error);
    } finally {
      setFormSubmitting(false);
    }
  };

  const getImagePath = (base64) => {
    return `data:image/jpeg;base64,${base64}`;
  };

  if (loading) {
    return (
      <main className="h-full w-full pt-[4.1rem] bg-themeColors-bgLightColor relative grid place-items-center">
        <Loader />
      </main>
    );
  }

  return (
    <section className=" p-12 pt-5">
      {/* Panel content */}
      <div className="flex flex-col">
        {damageData && damageData.length > 0 ? (
          <div className="relative px-10 mr-2 border-l border-solid border-l-themeColors-bgLightColor ml-8">
            <hr className="my-4 border-black opacity-10" />

            {damageData.map((dmg) => (
              <div key={dmg.id}>
                <div
                  className="relative -ml-10  pl-10 flex items-center justify-between"
                  onClick={() => handleAccordion(dmg.id)}
                >
                  {dmg.statusId === 4 && (
                    <div className="absolute top-0 left-0 w-12 h-12 grid place-items-center rounded-full text-white bg-themeColors-dangerColor text-2xl -translate-x-1/2 -translate-y-1">
                      <FaExclamationTriangle />
                    </div>
                  )}
                  {dmg.statusId === 5 && (
                    <div className="absolute top-0 left-0 w-12 h-12 grid place-items-center rounded-full text-white bg-themeColors-warningColor text-2xl -translate-x-1/2 -translate-y-1">
                      <FaWrench />
                    </div>
                  )}
                  {dmg.statusId === 6 && (
                    <div className="absolute top-0 left-0 w-12 h-12 grid place-items-center rounded-full text-white bg-themeColors-successColor text-2xl -translate-x-1/2 -translate-y-1">
                      <FaCheck />
                    </div>
                  )}
                  <div className="flex flex-col gap-2">
                    <p className="font-normal flex flex-col gap-1">
                      <span>{dmg.userName}</span>
                      <span className=" text-xs text-[#6c757d]">
                        {formatDate(dmg.createDate)}
                      </span>
                    </p>
                    <p
                      className=" text-themeColors-borderColor font-normal"
                      style={{ fontSize: "80%" }}
                    >
                      {t("category")}: {dmg.damageType.name}
                    </p>
                  </div>

                  <button className="btn btn-primary flex items-center">
                    {activeAccordion === dmg.id ? <BiMinus /> : <BiPlus />}
                  </button>
                </div>

                {/* Accordion panel */}
                <div
                  className="pt-6"
                  style={{
                    display: activeAccordion === dmg.id ? "block" : "none",
                  }}
                >
                  <hr className="my-4 border-black opacity-10" />
                  {dmg?.damageHistories?.map((item) => (
                    <React.Fragment key={`${item.id}-${item?.createDate}`}>
                      <div className="relative">
                        {item.statusId === 4 && (
                          <div className="absolute top-0 left-0 w-5 h-5 grid place-items-center rounded-full text-white bg-themeColors-dangerColor text-xs -translate-x-6">
                            <FaExclamationTriangle />
                          </div>
                        )}
                        {item.statusId === 5 && (
                          <div className="absolute top-0 left-0 w-5 h-5 grid place-items-center rounded-full text-white bg-themeColors-warningColor text-xs -translate-x-6">
                            <FaWrench />
                          </div>
                        )}
                        {item.statusId === 6 && (
                          <div className="absolute top-0 left-0 w-5 h-5 grid place-items-center rounded-full text-white bg-themeColors-successColor text-xs -translate-x-6">
                            <FaCheck />
                          </div>
                        )}
                        <div className="flex flex-col gap-2">
                          <p className="font-normal flex flex-col gap-1">
                            <span>{item?.assignedUser?.name}</span>
                            <span className=" text-xs text-[#6c757d]">
                              {formatDate(item?.createDate)}
                            </span>
                          </p>
                          {/* <p
                          className=" text-themeColors-borderColor font-normal"
                          style={{ fontSize: "80%" }}
                        >
                          {t("category")}: Damage
                        </p> */}
                        </div>
                        <p className="font-normal mb-4 mt-6">{item?.comment}</p>
                        {item.image && item.inage !== "" && (
                          <div className="w-full max-w-xl bg-themeColors-textColorLight">
                            <img src={item.image} className="w-full" />
                          </div>
                        )}
                      </div>
                      <hr className="my-4 border-black opacity-10" />
                    </React.Fragment>
                  ))}

                  {/* <hr className="my-4 border-black opacity-10" /> */}

                  {dmg.statusId !== 6 && (
                    <div className="flex flex-col gap-4 w-full p-6 rounded bg-[#F2F2F2] mt-10">
                      {userNames && userNames?.length > 0 && (
                        <select
                          name="owner"
                          id="owner"
                          className="input"
                          value={formData.assignedUserId}
                          onChange={(e) =>
                            setFormData((prev) => ({
                              ...prev,
                              assignedUserId: e.target.value,
                            }))
                          }
                        >
                          <option value={""}>Assign to</option>
                          {userNames?.map((item) => (
                            <option key={item.id} value={item.id}>
                              {item.name}
                            </option>
                          ))}
                        </select>
                      )}
                      <textarea
                        className="input w-full resize-none !h-auto"
                        placeholder="Comment"
                        value={formData.comment}
                        onChange={(e) =>
                          setFormData((prev) => ({
                            ...prev,
                            comment: e.target.value,
                          }))
                        }
                        rows={5}
                      ></textarea>
                      <label className="relative inline-flex items-center cursor-pointer gap-2">
                        <input
                          type="checkbox"
                          className="sr-only peer"
                          checked={formData.statusId === 6}
                          onChange={handleStatus}
                        />
                        <div className="w-11 h-6 bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-themeColors-bgColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-accentColor peer-checked:border-themeColors-accentColor peer-checked:after:bg-white"></div>
                        <span className=" whitespace-nowrap">
                          Problem solved
                        </span>
                      </label>
                      <button
                        disabled={formSubmitting}
                        onClick={handleSubmit}
                        className="btn btn-primary flex items-center w-fit mt-6 mx-auto"
                      >
                        {formSubmitting ? <LoaderSmall /> : <span>Save</span>}
                      </button>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        ) : (
          <NoResults />
        )}
      </div>
    </section>
  );
};

export default BinReport;
