'use client';

import React, { useState, useEffect } from 'react';
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
        <div className="relative z-10 flex min-h-screen items-center justify-center p-4 lg:p-8">
            <div className="absolute inset-0 bg-black/30 backdrop-blur-sm"></div>

            <div className="relative w-full max-w-md p-6 sm:p-8 bg-gray-900/90 rounded-2xl shadow-2xl border border-white/5">
                <header className="text-center mb-8">
                    <h1 className="text-3xl font-bold text-white tracking-tight">
                        {lang === 'tr' ? 'Yeni Şifrenizi Belirleyin' : 'Set Your New Password'}
                    </h1>
                </header>

                <main>
                    <form onSubmit={handleReset} className="space-y-5">
                        <div className="space-y-1">
                            <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="password">
                                {lang === 'tr' ? 'Yeni Şifre' : 'New Password'}
                            </label>
                            <div className="relative">
                                <input 
                                    className="w-full bg-gray-800/80 border-gray-700/50 rounded-xl focus:ring-2 focus:ring-primary/50 focus:border-primary text-white placeholder:text-gray-500 transition-all h-12 pl-4 pr-12 outline-none" 
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
                                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-white transition-colors focus:outline-none p-1"
                                >
                                    <span className="material-symbols-outlined text-xl">
                                        {showPassword ? "visibility_off" : "visibility"}
                                    </span>
                                </button>
                            </div>
                        </div>

                        <div className="space-y-1">
                            <label className="block text-sm font-semibold text-gray-300 ml-1" htmlFor="repeatPassword">
                                {lang === 'tr' ? 'Yeni Şifre (Tekrar)' : 'New Password (Repeat)'}
                            </label>
                            <input 
                                className="w-full bg-gray-800/80 border-gray-700/50 rounded-xl focus:ring-2 focus:ring-primary/50 focus:border-primary text-white placeholder:text-gray-500 transition-all h-12 px-4 outline-none" 
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
                            className="w-full bg-primary text-white font-bold py-4 px-6 rounded-xl shadow-lg shadow-primary/20 flex items-center justify-center gap-2 mt-2 hover:bg-primary/90 transition-all active:scale-[0.98] disabled:opacity-50"
                            type="submit"
                            disabled={loading}
                        >
                            {loading ? (
                                <div className="h-5 w-5 border-2 border-white/20 border-t-white rounded-full animate-spin"></div>
                            ) : (lang === 'tr' ? 'Şifreyi Güncelle' : 'Update Password')}
                        </button>
                    </form>
                </main>
            </div>
        </div>
    );
}
