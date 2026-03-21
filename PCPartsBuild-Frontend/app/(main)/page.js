'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { translations } from '@/lib/translations';

export default function HomePage() {
    const [lang, setLang] = useState('tr');

    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);

        const handleStorageChange = () => {
            setLang(localStorage.getItem('lang') || 'tr');
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, []);

    const t = translations[lang];

    return (
        <div className="relative flex flex-1 flex-col">
            {/* Background Video - Homepage ONLY */}
            <div className="fixed inset-0 z-[-1] overflow-hidden">
                <video autoPlay className="h-full w-full object-cover brightness-50" loop muted playsInline>
                    <source src="/Videos/Video2.webm" type="video/webm" />
                    Your browser does not support the video tag.
                </video>
            </div>

            <main className="flex flex-1 flex-col">
                <div className="flex flex-1 items-center justify-center py-20 text-center">
                    <div className="flex flex-col items-center gap-6">
                        <h1 className="text-white text-4xl font-black leading-tight tracking-[-0.033em] md:text-6xl" data-i18n="hero-title">
                            {t["hero-title"]}
                        </h1>
                        <h2 className="max-w-2xl text-white/80 text-base font-normal leading-normal md:text-lg" data-i18n="hero-subtitle">
                            {t["hero-subtitle"]}
                        </h2>
                        <Link href="/pts" className="flex min-w-[84px] max-w-[480px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 px-5 bg-primary text-white text-base font-bold leading-normal tracking-[0.015em] transition-transform hover:scale-105">
                            <span className="truncate" data-i18n="hero-button">{t["hero-button"]}</span>
                        </Link>
                    </div>
                </div>
                
                <div className="flex flex-col gap-10 py-16 @container">
                    <div className="flex flex-col gap-4 text-center">
                        <h1 className="text-white tracking-light text-3xl font-bold leading-tight @[480px]:text-4xl @[480px]:font-black @[480px]:leading-tight @[480px]:tracking-[-0.033em]" data-i18n="features-main-title">
                            {t["features-main-title"]}
                        </h1>
                        <p className="text-white/80 text-base font-normal leading-normal max-w-3xl mx-auto" data-i18n="features-main-subtitle">
                            {t["features-main-subtitle"]}
                        </p>
                    </div>
                    
                    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                        <div className="flex flex-1 flex-col gap-3 rounded-xl border border-white/10 bg-black/20 p-6 backdrop-blur-sm transition-transform hover:-translate-y-1">
                            <div className="text-primary">
                                <span className="material-symbols-outlined text-3xl">verified</span>
                            </div>
                            <div className="flex flex-col gap-1">
                                <h2 className="text-white text-lg font-bold leading-tight" data-i18n="feature-1-title">
                                    {t["feature-1-title"]}
                                </h2>
                                <p className="text-white/70 text-sm font-normal leading-normal" data-i18n="feature-1-desc">
                                    {t["feature-1-desc"]}
                                </p>
                            </div>
                        </div>
                        
                        <div className="flex flex-1 flex-col gap-3 rounded-xl border border-white/10 bg-black/20 p-6 backdrop-blur-sm transition-transform hover:-translate-y-1">
                            <div className="text-primary">
                                <span className="material-symbols-outlined text-3xl">auto_fix_high</span>
                            </div>
                            <div className="flex flex-col gap-1">
                                <h2 className="text-white text-lg font-bold leading-tight" data-i18n="feature-2-title">
                                    {t["feature-2-title"]}
                                </h2>
                                <p className="text-white/70 text-sm font-normal leading-normal" data-i18n="feature-2-desc">
                                    {t["feature-2-desc"]}
                                </p>
                            </div>
                        </div>
                        
                        <div className="flex flex-1 flex-col gap-3 rounded-xl border border-white/10 bg-black/20 p-6 backdrop-blur-sm transition-transform hover:-translate-y-1">
                            <div className="text-primary">
                                <span className="material-symbols-outlined text-3xl">database</span>
                            </div>
                            <div className="flex flex-col gap-1">
                                <h2 className="text-white text-lg font-bold leading-tight" data-i18n="feature-3-title">
                                    {t["feature-3-title"]}
                                </h2>
                                <p className="text-white/70 text-sm font-normal leading-normal" data-i18n="feature-3-desc">
                                    {t["feature-3-desc"]}
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
}
