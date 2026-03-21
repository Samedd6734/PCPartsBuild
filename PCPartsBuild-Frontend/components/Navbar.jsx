'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';

const translations = {
    tr: {
        "nav-wizard": "PC Toplama Sihirbazı",
        "nav-compatibility": "Parça Uyumluluk Kontrolü",
        "nav-info": "PC Bileşenleri",
        "auth-signup": "Kayıt Ol",
        "auth-login": "Giriş Yap",
        "auth-welcome": "Hoş geldin,",
        "nav-profile": "Hesabım",
        "auth-logout": "Çıkış Yap",
    },
    en: {
        "nav-wizard": "PC Build Wizard",
        "nav-compatibility": "Part Compatibility Check",
        "nav-info": "PC Components",
        "auth-signup": "Sign Up",
        "auth-login": "Log In",
        "auth-welcome": "Welcome,",
        "nav-profile": "My Account",
        "auth-logout": "Log Out",
    }
};

export default function Navbar() {
    const [lang, setLang] = useState('tr');
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const [isLangDropdownOpen, setIsLangDropdownOpen] = useState(false);
    const [isProfileDropdownOpen, setIsProfileDropdownOpen] = useState(false);
    const [loggedInUser, setLoggedInUser] = useState(null);

    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);
        document.documentElement.lang = savedLang;
        
        const user = localStorage.getItem('loggedInUser');
        if (user) setLoggedInUser(user);
    }, []);

    const toggleLanguage = (newLang) => {
        setLang(newLang);
        localStorage.setItem('lang', newLang);
        document.documentElement.lang = newLang;
        setIsLangDropdownOpen(false);
    };

    const handleLogout = () => {
        localStorage.removeItem('loggedInUser');
        setLoggedInUser(null);
        window.location.reload();
    };

    const t = translations[lang];

    return (
        <header className="relative w-full border-b border-white/10 z-50">
            <div className="h-20 flex items-center justify-between">
                
                <div className="flex items-center gap-4 text-white z-20">
                    <Link href="/">
                        <img src="/Images/logo.webp" alt="PCPartsBuild Logo" className="h-10 sm:h-14 w-auto object-contain" />
                    </Link>
                </div>

                <nav className="hidden lg:flex items-center gap-9">
                    <Link className="text-white/80 text-sm font-medium leading-normal transition-colors hover:text-white" href="/pts">
                        {t["nav-wizard"]}
                    </Link>
                    <Link className="text-white/80 text-sm font-medium leading-normal transition-colors hover:text-white" href="/puk">
                        {t["nav-compatibility"]}
                    </Link>
                    <Link className="text-white/80 text-sm font-medium leading-normal transition-colors hover:text-white" href="/pbg">
                        {t["nav-info"]}
                    </Link>
                </nav>

                <div className="flex items-center gap-2 sm:gap-4">
                    
                    {!loggedInUser && (
                        <>
                            <div id="mobile-auth-buttons" className="flex lg:hidden items-center gap-2">
                                <Link href="/kaydol" className="px-3 py-2 bg-primary hover:bg-sky-600 text-white text-xs sm:text-sm font-bold rounded-lg transition-colors shadow-lg shadow-primary/20 whitespace-nowrap">
                                    {t["auth-signup"]}
                                </Link>
                                <Link href="/giris" className="px-3 py-2 bg-white/10 hover:bg-white/20 text-white text-xs sm:text-sm font-medium rounded-lg transition-colors whitespace-nowrap">
                                    {t["auth-login"]}
                                </Link>
                            </div>

                            <div id="desktop-auth-buttons" className="hidden lg:flex items-center gap-2">
                                <Link href="/kaydol" className="flex min-w-[84px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-10 px-4 bg-primary text-white text-sm font-bold leading-normal tracking-[0.015em] transition-transform hover:scale-105">
                                    <span className="truncate">{t["auth-signup"]}</span>
                                </Link>
                                <Link href="/giris" className="flex min-w-[84px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-10 px-4 bg-white/10 text-white text-sm font-bold leading-normal tracking-[0.015em] transition-colors hover:bg-white/20">
                                    <span className="truncate">{t["auth-login"]}</span>
                                </Link>
                            </div>
                        </>
                    )}

                    <div id="languageSelector" className="relative hidden lg:block" onMouseLeave={() => setIsLangDropdownOpen(false)}>
                        <button 
                            id="languageButton" 
                            className="flex cursor-pointer items-center justify-center overflow-hidden rounded-lg h-10 w-10 bg-white/10 text-white text-sm font-bold leading-normal tracking-[0.015em] transition-colors hover:bg-white/20"
                            onClick={() => setIsLangDropdownOpen(!isLangDropdownOpen)}
                        >
                            <span className="material-symbols-outlined text-xl">language</span>
                        </button>
                        {isLangDropdownOpen && (
                            <div id="languageDropdown" className="absolute right-0 top-full pt-1 w-32 bg-white/5 backdrop-blur-md border border-white/10 rounded-lg shadow-xl overflow-hidden z-20">
                                <button onClick={() => toggleLanguage('tr')} className="language-option block w-full text-left px-4 py-2 text-sm text-white/90 font-medium hover:bg-primary/50 hover:text-white transition-colors duration-150">Türkçe (TR)</button>
                                <button onClick={() => toggleLanguage('en')} className="language-option block w-full text-left px-4 py-2 text-sm text-white/90 font-medium hover:bg-primary/50 hover:text-white transition-colors duration-150">English (EN)</button>
                            </div>
                        )}
                    </div>

                    {loggedInUser && (
                        <div id="account-section" className="relative" onMouseLeave={() => setIsProfileDropdownOpen(false)}>
                            <button 
                                id="profileButton" 
                                className="flex material-icons-outlined cursor-pointer items-center justify-center overflow-hidden rounded-lg h-10 w-10 bg-white/10 text-white text-xl font-bold leading-normal tracking-[0.015em] transition-colors hover:bg-white/20"
                                onClick={() => setIsProfileDropdownOpen(!isProfileDropdownOpen)}
                            >
                                <span>account_circle</span>
                            </button>
                            {isProfileDropdownOpen && (
                                <div id="profileDropdown" className="absolute right-0 top-full pt-1 w-48 bg-white/5 backdrop-blur-md border border-white/10 rounded-lg shadow-xl overflow-hidden z-20">
                                    <div className="px-4 py-3 border-b border-white/10">
                                        <p className="text-sm text-white/70">{t["auth-welcome"]}</p>
                                        <p id="welcome-username" className="text-sm font-medium text-white truncate">{loggedInUser}</p>
                                    </div>
                                    <Link href="/hesabim" className="block px-4 py-2 text-sm text-white/90 font-medium hover:bg-primary/50 hover:text-white transition-colors duration-150">
                                        {t["nav-profile"]}
                                    </Link>
                                    <button onClick={handleLogout} id="logout-button" className="block w-full text-left px-4 py-2 text-sm text-red-400 font-medium hover:bg-red-500/50 hover:text-white transition-colors duration-150">
                                        {t["auth-logout"]}
                                    </button>
                                </div>
                            )}
                        </div>
                    )}

                    <button 
                        id="mobile-menu-btn" 
                        className="lg:hidden p-2 text-white hover:text-primary focus:outline-none transition-colors ml-2"
                        onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                    >
                        <span className="material-icons-round text-3xl">{isMobileMenuOpen ? 'close' : 'menu'}</span>
                    </button>
                </div>
            </div>

            {isMobileMenuOpen && (
                <div id="mobile-menu" className="absolute top-full left-0 w-full bg-[#111c22] border-b border-white/10 shadow-xl py-6 px-6 flex flex-col gap-4 lg:hidden origin-top animate-fade-in-down z-50">
                    <Link className="block py-2 text-lg font-medium text-white/90 hover:text-primary transition-colors" href="/pts" onClick={() => setIsMobileMenuOpen(false)}>
                        {t["nav-wizard"]}
                    </Link>
                    <Link className="block py-2 text-lg font-medium text-white/90 hover:text-primary transition-colors" href="/puk" onClick={() => setIsMobileMenuOpen(false)}>
                        {t["nav-compatibility"]}
                    </Link>
                    <Link className="block py-2 text-lg font-medium text-white/90 hover:text-primary transition-colors" href="/pbg" onClick={() => setIsMobileMenuOpen(false)}>
                        {t["nav-info"]}
                    </Link>
                    
                    <div className="h-px bg-white/10 my-2"></div>

                    <div className="flex flex-col gap-3">
                        <div className="flex items-center justify-between mt-2 px-1">
                            <button 
                                id="mobile-lang-toggle" 
                                className="flex items-center gap-2 text-white/60 hover:text-white transition-colors"
                                onClick={() => toggleLanguage(lang === 'tr' ? 'en' : 'tr')}
                            >
                                <span className="material-icons-round">language</span>
                                <span id="mobile-lang-text">{lang === 'tr' ? 'Türkçe' : 'English'}</span>
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </header>
    );
}
