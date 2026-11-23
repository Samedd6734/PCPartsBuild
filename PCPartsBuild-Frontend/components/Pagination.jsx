'use client';

import React from 'react';
import { useRouter, usePathname, useSearchParams } from 'next/navigation';

export default function Pagination({ totalPages, currentPage }) {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const handlePageChange = (newPage) => {
    if (newPage < 1 || newPage > totalPages) return;
    
    // Create new URLSearchParams to safely update the URL
    const params = new URLSearchParams(searchParams.toString());
    params.set('page', newPage.toString());
    
    // Push the new URL without triggering a full page reload (Next.js app router default shallow push)
    router.push(`${pathname}?${params.toString()}`, { scroll: false });
  };

  if (totalPages <= 1) return null;

  return (
    <div className="mt-10 flex justify-center gap-2 items-center">
      <button 
        onClick={() => handlePageChange(currentPage - 1)} 
        disabled={currentPage <= 1}
        className={`w-10 h-10 rounded-lg border flex items-center justify-center transition-colors 
          ${currentPage <= 1 ? 'opacity-50 cursor-not-allowed border-gray-200 dark:border-gray-800 text-gray-400' : 'dark:border-gray-700 hover:border-primary text-gray-700 dark:text-gray-300'}`}
      >
        <span className="material-symbols-outlined">chevron_left</span>
      </button>
      
      <span className="flex items-center px-4 font-bold text-gray-500 text-sm whitespace-nowrap">
        {currentPage} / {totalPages}
      </span>
      
      <button 
        onClick={() => handlePageChange(currentPage + 1)} 
        disabled={currentPage >= totalPages}
        className={`w-10 h-10 rounded-lg border flex items-center justify-center transition-colors 
          ${currentPage >= totalPages ? 'opacity-50 cursor-not-allowed border-gray-200 dark:border-gray-800 text-gray-400' : 'dark:border-gray-700 hover:border-primary text-gray-700 dark:text-gray-300'}`}
      >
        <span className="material-symbols-outlined">chevron_right</span>
      </button>
    </div>
  );
}
