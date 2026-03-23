'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { useSearchParams, useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import Swal from 'sweetalert2';

export default function ResetPasswordPage() {
    const searchParams = useSearchParams();
    const router = useRouter();
    const [lang, setLang] = useState('tr');
    const [loading, setLoading] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [formData, setFormData] = useState({
        password: '',
        repeatPassword: ''
    });

    const email = searchParams.get('email');
    const token = searchParams.get('token')?.replace(/ /g, '+'); // Fix token if needed

    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);

        if (!email || !token) {
            Swal.fire({
                title: lang === 'tr' ? 'Hatalı Link' : 'Invalid Link',
                text: lang === 'tr' ? 'Lütfen şifremi unuttum işlemini tekrar başlatın.' : 'Please restart the forgot password process.',
                icon: 'error',
                background: '#1f2937', color: '#fff', confirmButtonColor: '#d33'
            }).then(() => {
                router.push('/sifremi-unuttum');
            });
        }
    }, [email, token, lang, router]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleReset = async (e) => {
        e.preventDefault();

        if (formData.password !== formData.repeatPassword) {
            Swal.fire({
                title: lang === 'tr' ? 'Hata' : 'Error', 
                text: lang === 'tr' ? 'Şifreler eşleşmiyor!' : 'Passwords do not match!',
                icon: 'warning', background: '#1f2937', color: '#fff', confirmButtonColor: '#16a3b2'
            });
            return;
        }

        setLoading(true);

        try {
            const response = await api.post('auth/reset-password', {
                email: email,
                token: token,
                newPassword: formData.password
            });

            if (response.ok) {
                Swal.fire({
                    title: lang === 'tr' ? 'Şifre Değiştirildi!' : 'Password Changed!',
                    text: lang === 'tr' ? 'Yeni şifrenizle giriş yapabilirsiniz. Yönlendiriliyorsunuz...' : 'You can log in with your new password. Redirecting...',
                    icon: 'success',
                    background: '#1f2937', color: '#fff', confirmButtonColor: '#16a3b2',
                    timer: 2000, timerProgressBar: true
                }).then(() => {
                    router.push('/giris');
                });
            } else {
                const result = await response.json().catch(() => ({}));
                Swal.fire({
                    title: lang === 'tr' ? 'İşlem Başarısız' : 'Operation Failed',
                    text: result.message || (lang === 'tr' ? 'Şifre sıfırlanamadı.' : 'Could not reset password.'),
                    icon: 'error',
                    background: '#1f2937', color: '#fff', confirmButtonColor: '#d33'
                });
            }
        } catch (error) {
            console.error('Reset Password Error:', error);
            Swal.fire({
                title: lang === 'tr' ? 'Bağlantı Hatası' : 'Connection Error', 
                text: lang === 'tr' ? 'Sunucuya ulaşılamadı.' : 'Could not reach server.',
                icon: 'error', background: '#1f2937', color: '#fff'
            });
        } finally {
            setLoading(false);
        }
    };

    if (!email || !token) return null;

    return (
        <div className="font-auth bg-background-dark font-display text-gray-300 transition-default overflow-hidden min-h-screen relative">
            {/* 1. Background Video - SHARP & CLEAR */}
            <video autoPlay id="bg-video" loop muted playsInline className="fixed inset-0 w-full h-full object-cover z-0 brightness-[0.5]">
                <source src="/Videos/Video1.webm" type="video/webm" />
                Your browser does not support the video tag.
            </video>
            
            {/* 2. Top Navigation - SHARED */}
            <Link 
                href="/" 
                className="absolute top-4 left-4 z-50 w-12 h-12 rounded-full bg-black/40 border border-white/10 flex items-center justify-center text-white hover:bg-black/60 transition-all hover:scale-105"
                title="Home"
            >
                <span className="material-symbols-outlined text-xl"> home </span>
            </Link>

            {/* 3. Main Split Container - SHARP SYNC */}
            <div className="relative z-10 min-h-screen grid grid-cols-1 lg:grid-cols-2 bg-transparent">
                
                {/* Branding Side (Lg only) */}
                <div className="hidden lg:flex flex-col items-center justify-center text-center px-8 bg-transparent animate-fade-in">
                    <img src="/Images/logo.webp" alt="PCPartsBuild Logo" className="h-48 drop-shadow-2xl" />
                    <p className="text-gray-300 mt-6 max-w-sm text-lg font-medium tracking-wide drop-shadow-lg">
                        {lang === 'tr' ? 'Hayalinizdeki Bilgisayarı hassasiyet ve güvenle tasarlayın.' : 'Design your dream PC with precision and confidence.'}
                    </p>
                </div>

                {/* Right Form Side - SHARP THEME (No Blur) */}
                <div className="relative flex items-center justify-center p-4 lg:p-12 bg-background-dark/95 border-l border-gray-800 shadow-2xl">
                    
                    <div className="relative w-full max-w-md p-6 sm:p-10 bg-gray-900/40 rounded-[2.5rem] border border-white/5 animate-fade-in transition-all">
                        <header className="text-center lg:text-left mb-8">
                            <h1 className="text-3xl font-bold text-white tracking-tight">
                                {lang === 'tr' ? 'Yeni Şifre Belirleyin' : 'Set New Password'}
                            </h1>
                            <p className="text-gray-400 mt-2 text-sm italic">
                                {lang === 'tr' ? 'Güvenliğiniz için güçlü bir şifre seçin.' : 'Choose a strong password for your security.'}
                            </p>
                        </header>

                        <main>
                            <form onSubmit={handleReset} className="space-y-6">
                                <div className="space-y-1.5">
                                    <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="password">
                                        {lang === 'tr' ? 'Yeni Şifre' : 'New Password'}
                                    </label>
                                    <div className="relative">
                                        <input 
                                            className="w-full bg-white/5 border border-white/10 rounded-2xl focus:ring-2 focus:ring-primary focus:border-primary text-white placeholder:text-gray-600 transition-all h-14 pl-5 pr-12 outline-none active:bg-white/10 focus:bg-white/10" 
                                            id="password" 
                                            name="password" 
                                            placeholder="••••••••" 
                                            type={showPassword ? "text" : "password"}
                                            required
                                            value={formData.password}
                                            onChange={handleChange}
                                        />
                                        <button 
                                            type="button" 
                                            onClick={() => setShowPassword(!showPassword)} 
                                            className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-500 hover:text-white transition-colors p-1"
                                        >
                                            <span className="material-symbols-outlined text-xl">
                                                {showPassword ? "visibility_off" : "visibility"}
                                            </span>
                                        </button>
                                    </div>
                                </div>

                                <div className="space-y-1.5">
                                    <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="repeatPassword">
                                        {lang === 'tr' ? 'Yeni Şifre (Tekrar)' : 'New Password (Repeat)'}
                                    </label>
                                    <input 
                                        className="w-full bg-white/5 border border-white/10 rounded-2xl focus:ring-2 focus:ring-primary focus:border-primary text-white placeholder:text-gray-600 transition-all h-14 px-5 outline-none active:bg-white/10 focus:bg-white/10" 
                                        id="repeatPassword" 
                                        name="repeatPassword" 
                                        placeholder="••••••••" 
                                        type={showPassword ? "text" : "password"}
                                        required
                                        value={formData.repeatPassword}
                                        onChange={handleChange}
                                    />
                                </div>

                                <button 
                                    className="w-full bg-primary text-white font-bold py-5 px-6 rounded-2xl shadow-xl shadow-primary/20 flex items-center justify-center gap-2 mt-4 hover:bg-primary/90 transition-all active:scale-[0.98] disabled:opacity-50"
                                    type="submit"
                                    disabled={loading}
                                >
                                    {loading ? (
                                        <div className="h-5 w-5 border-2 border-white/20 border-t-white rounded-full animate-spin"></div>
                                    ) : (lang === 'tr' ? 'Şifreyi Güncelle' : 'Update Password')}
                                </button>
                            </form>
                        </main>

                        <footer className="text-center mt-10 pt-8 border-t border-white/5">
                            <Link className="text-sm font-bold text-primary hover:underline transition-all" href="/giris">
                                {lang === 'tr' ? 'Giriş Sayfasına Dön' : 'Back to Login'}
                            </Link>
                        </footer>
                    </div>

                    {/* Shared Page Footer */}
                    <footer className="absolute bottom-6 w-full text-center px-4"> 
                        <p className="text-xs text-gray-500 font-medium tracking-wide"> 
                            © 2025 PCPartsBuild. {lang === 'tr' ? 'Tüm Hakları Saklıdır.' : 'All Rights Reserved.'}
                            <span className="mx-2 opacity-30">|</span> 
                            {lang === 'tr' ? 'Bize Ulaşın:' : 'Contact Us:'}{" "}
                            <a className="text-primary hover:underline" href="mailto:pcpartsbuild@gmail.com">pcpartsbuild@gmail.com</a>
                        </p>
                    </footer>
                </div>
            </div>
        </div>
    );
}
