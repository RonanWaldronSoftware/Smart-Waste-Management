import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";

const Pagination = ({
  totalItems,
  itemsPerPage,
  currentPage,
  setCurrentPage,
  onPageChange,
}) => {
  const { t } = useTranslation("translation");
  const totalPages = Math.ceil(totalItems / itemsPerPage);
  const maxPageButtons = 5;

  useEffect(() => {
    setCurrentPage(1);
  }, [totalItems, itemsPerPage]);

  const handleFirstPage = () => {
    setCurrentPage(1);
  };

  const handleLastPage = () => {
    setCurrentPage(totalPages);
  };

  const handleNextPage = () => {
    setCurrentPage((prevPage) => Math.min(prevPage + 1, totalPages));
  };

  const handlePrevPage = () => {
    setCurrentPage((prevPage) => Math.max(prevPage - 1, 1));
  };

  const handlePageClick = (pageNumber) => {
    if (pageNumber !== currentPage) {
      onPageChange(pageNumber);
    }
  };

  const getPageNumbersRange = () => {
    const middlePage = Math.ceil(maxPageButtons / 2);
    let startPage = currentPage - middlePage + 1;
    let endPage = currentPage + middlePage - 1;

    if (startPage < 1) {
      startPage = 1;
      endPage = maxPageButtons;
    }

    if (endPage > totalPages) {
      endPage = totalPages;
      startPage = totalPages - maxPageButtons + 1;
      if (startPage < 1) {
        startPage = 1;
      }
    }

    return Array.from(
      { length: endPage - startPage + 1 },
      (_, i) => startPage + i
    );
  };

  const pageNumbers = getPageNumbersRange();
  return (
    <ul className="pagination ml-auto">
      <li>
        <button
          className={`pagination-btn`}
          disabled={currentPage === 1}
          onClick={handleFirstPage}
        >
          {t("first")}
        </button>
      </li>
      <li>
        <button
          className={`pagination-btn`}
          disabled={currentPage === 1}
          onClick={handlePrevPage}
        >
          {t("previous")}
        </button>
      </li>

      {pageNumbers.map((pageNumber) => (
        <li key={pageNumber}>
          <button
            className={`pagination-btn ${
              currentPage === pageNumber ? "active" : ""
            }`}
            onClick={() => handlePageClick(pageNumber)}
          >
            {pageNumber}
          </button>
        </li>
      ))}

      <li>
        <button
          className={`pagination-btn`}
          disabled={currentPage === totalPages}
          onClick={handleNextPage}
        >
          {t("next")}
        </button>
      </li>
      <li
        className={`page-item ${currentPage === totalPages ? "disabled" : ""}`}
      >
        <button
          className={`pagination-btn`}
          disabled={currentPage === totalPages}
          onClick={handleLastPage}
        >
          {t("last")}
        </button>
      </li>
    </ul>
  );
};

export default Pagination;
